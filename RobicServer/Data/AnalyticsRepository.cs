using System.Collections.Generic;
using System.Linq;
using RobicServer.Helpers;
using RobicServer.Interfaces;
using RobicServer.Models;

namespace RobicServer.Data
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        struct ExerciseProgressValue
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public double Total { get; set; }
            public int NumberOfSessions { get; set; }
        };

        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;

        public AnalyticsRepository(IMongoRepository<Exercise> exerciseContext, IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext)
        {
            _exerciseContext = exerciseContext;
            _exerciseDefinitionContext = exerciseDefinitionContext;
        }

        // Gets exercises and exercise definitions based on user ID in claims
        public void GetUserExercises(string userId, out List<ExerciseDefiniton> userExerciseDefinitions, out List<Exercise> userExercises)
        {

            userExerciseDefinitions = _exerciseDefinitionContext.AsQueryable().Where(e => e.User == userId).ToList();
            var exercises = new List<Exercise>();
            userExerciseDefinitions.AsParallel().ForAll(def =>
            {
                IQueryable<Exercise> defExercises = _exerciseContext.AsQueryable().Where(e => e.Definition == def.Id);
                if (defExercises.Count() != 0 && defExercises != null)
                    lock (exercises)
                        exercises.AddRange(defExercises);
            });
            userExercises = exercises;
        }

        public List<AnalyticsItem> GetExerciseFrequency(List<ExerciseDefiniton> exerciseDefinitions)
        {
            // Increment muscle group by occurance
            List<AnalyticsItem> exerciseFrequency = new List<AnalyticsItem>();
            foreach (var def in exerciseDefinitions)
            {
                if (def.History != null)
                {
                    exerciseFrequency.Add(new AnalyticsItem()
                    {
                        Marker = def.Title,
                        Count = def.History.Count
                    });
                }
            }
            exerciseFrequency.Sort((a, b) => a.Count < b.Count ? 1 : -1);
            return exerciseFrequency;
        }

        public List<AnalyticsItem> GetExerciseProgress(List<Exercise> exercises, List<ExerciseDefiniton> exerciseDefinitions)
        {
            Dictionary<string, ExerciseProgressValue> exerciseProgressDict = new Dictionary<string, ExerciseProgressValue>();
            foreach (var e in exercises)
            {
                string defId = e.Definition;
                if (exerciseProgressDict.ContainsKey(defId))
                {
                    ExerciseProgressValue val = exerciseProgressDict[defId];
                    val.Total += ExerciseUtilities.GetNetExerciseValue(e);
                    val.NumberOfSessions += 1;
                    exerciseProgressDict[defId] = val;
                }
                else
                {
                    ExerciseDefiniton def = exerciseDefinitions.Find(d => d.Id == defId);
                    exerciseProgressDict.Add(defId, new ExerciseProgressValue
                    {
                        Id = def.Id,
                        Title = def.Title,
                        Total = ExerciseUtilities.GetNetExerciseValue(e),
                        NumberOfSessions = 1
                    });
                }
            }

            // Convert dict to analytics list
            List<AnalyticsItem> exerciseProgress = new List<AnalyticsItem>();
            foreach (KeyValuePair<string, ExerciseProgressValue> progressItem in exerciseProgressDict)
            {
                ExerciseDefiniton def = exerciseDefinitions.Find(d => d.Id == progressItem.Value.Id);
                Exercise firstExercise = exercises.Find(ex => ex.Id == def.History?.FirstOrDefault());

                double progressPercent = 0.0;
                if (firstExercise != null)
                {
                    double initialNetValue = ExerciseUtilities.GetNetExerciseValue(firstExercise);

                    // Progress percent = average net value - initial net value
                    progressPercent = (progressItem.Value.Total / progressItem.Value.NumberOfSessions) - initialNetValue;
                }


                exerciseProgress.Add(new AnalyticsItem
                {
                    Marker = progressItem.Value.Title,
                    Count = progressPercent,
                });
            }
            exerciseProgress.Sort((a, b) => a.Count < b.Count ? 1 : -1);
            return exerciseProgress;
        }

        public List<AnalyticsItem> GetMuscleGroupFrequency(List<Exercise> exercises, List<ExerciseDefiniton> exerciseDefinitions)
        {
            // Increment muscle group by occurance
            Dictionary<string, int> muscleGroupFrequency = new Dictionary<string, int>();
            exercises.ForEach(e =>
            {
                ExerciseDefiniton exerciseDefiniton = exerciseDefinitions.Find(def => def.Id == e.Definition);
                if (exerciseDefiniton != null && exerciseDefiniton.PrimaryMuscleGroup != null)
                {
                    exerciseDefiniton.PrimaryMuscleGroup.ToList().ForEach(m =>
                 {
                     if (muscleGroupFrequency.ContainsKey(m))
                     {
                         muscleGroupFrequency[m] += 1;
                     }
                     else
                     {
                         muscleGroupFrequency.Add(m, 1);
                     }
                 });
                }
            });

            // Convert dictionary to analytics list
            List<AnalyticsItem> muscleGroupFrequencyList = new List<AnalyticsItem>();
            foreach (KeyValuePair<string, int> muscleGroup in muscleGroupFrequency)
            {
                muscleGroupFrequencyList.Add(new AnalyticsItem()
                {
                    Marker = muscleGroup.Key,
                    Count = muscleGroup.Value
                });
            }
            muscleGroupFrequencyList.Sort((a, b) => a.Count < b.Count ? 1 : -1);
            return muscleGroupFrequencyList;
        }
    }
}