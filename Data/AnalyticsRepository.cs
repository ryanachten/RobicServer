using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RobicServer.Helpers;
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

        private readonly IMongoRepository<Exercise> _exerciseRepo;
        private readonly IMongoRepository<ExerciseDefinition> _exerciseDefinitionRepo;
        private IEnumerable<ExerciseDefinition> _userExerciseDefinitions;
        private List<Exercise> _userExercises;

        public AnalyticsRepository(
            IMongoRepository<Exercise> exerciseRepo,
            IMongoRepository<ExerciseDefinition> exerciseDefinitionRepo
        )
        {
            _exerciseRepo = exerciseRepo;
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
        }
        public async Task<Analytics> GetUserAnalytics(string userId)
        {
            GetUserExercises(userId);
            List<AnalyticsItem> muscleGroupFrequency = this.GetMuscleGroupFrequency();
            List<AnalyticsItem> exerciseFrequency = this.GetExerciseFrequency();

            return new Analytics()
            {
                ExerciseFrequency = exerciseFrequency,
                MostFrequentExercise = exerciseFrequency.FirstOrDefault(),
                ExerciseProgress = this.GetExerciseProgress(),
                MuscleGroupFrequency = muscleGroupFrequency,
                MostFrequentMuscleGroup = muscleGroupFrequency.FirstOrDefault(),
            };
        }

        private List<AnalyticsItem> GetExerciseProgress()
        {
            Dictionary<string, ExerciseProgressValue> exerciseProgressDict = new Dictionary<string, ExerciseProgressValue>();
            foreach (var e in _userExercises)
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
                    ExerciseDefinition def = _userExerciseDefinitions.FirstOrDefault(d => d.Id == defId);
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
                ExerciseDefinition def = _userExerciseDefinitions.FirstOrDefault(d => d.Id == progressItem.Value.Id);
                Exercise firstExercise = _userExercises.FirstOrDefault(ex => ex.Id == def.History?.FirstOrDefault());

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

        private List<AnalyticsItem> GetExerciseFrequency()
        {
            // Increment muscle group by occurance
            List<AnalyticsItem> exerciseFrequency = _userExerciseDefinitions.Select(
                (def) =>
                {
                    return new AnalyticsItem
                    {
                        Marker = def.Title,
                        Count = def.History.Count
                    };
                }
            ).OrderByDescending(a => a.Count).ToList();
            return exerciseFrequency;
        }

        private List<AnalyticsItem> GetMuscleGroupFrequency()
        {
            // Increment muscle group by occurance
            Dictionary<string, int> muscleGroupFrequency = new Dictionary<string, int>();
            _userExercises.ToList().ForEach(e =>
            {
                ExerciseDefinition exerciseDefiniton = _userExerciseDefinitions.FirstOrDefault(def => def.Id == e.Definition);
                if (exerciseDefiniton != null && exerciseDefiniton.PrimaryMuscleGroup != null)
                {
                    exerciseDefiniton.PrimaryMuscleGroup.ForEach(m =>
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

        // Gets exercises and exercise definitions based on user ID in claimss
        private void GetUserExercises(string userId)
        {
            _userExerciseDefinitions = _exerciseDefinitionRepo.FilterBy(e => e.User == userId).ToList();
            var exericseIds = _userExerciseDefinitions.Select(e => e.Id);
            _userExercises = _exerciseRepo.FilterBy(e => exericseIds.Contains(e.Definition)).ToList();
        }
    }
}