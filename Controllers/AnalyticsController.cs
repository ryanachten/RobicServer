using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using RobicServer.Helpers;

namespace RobicServer.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        struct ExerciseProgressValue
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public double Total { get; set; }
            public int NumberOfSessions { get; set; }
        };

        private readonly IMongoRepository<Exercise> _exerciseRepo;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionRepo;
        private List<ExerciseDefiniton> _userExerciseDefinitions;
        private List<Exercise> _userExercises;

        public AnalyticsController(IMongoRepository<Exercise> exerciseRepo, IMongoRepository<ExerciseDefiniton> exerciseDefinitionRepo)
        {
            _exerciseRepo = exerciseRepo;
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
        }

        [HttpGet]
        public Analytics Get()
        {
            this.GetUserExercises();
            List<AnalyticsItem> muscleGroupFrequency = this.GetMuscleGroupFrequency();
            List<AnalyticsItem> exerciseFrequency = this.GetExerciseFrequency();

            Analytics analytics = new Analytics()
            {
                ExerciseFrequency = exerciseFrequency,
                MostFrequentExercise = this.GetMostFrequentExercise(exerciseFrequency),
                ExerciseProgress = this.GetExerciseProgress(),
                MuscleGroupFrequency = muscleGroupFrequency,
                MostFrequentMuscleGroup = this.GetMostFrequentMuscleGroup(muscleGroupFrequency),
            };

            return analytics;
        }

        // Gets exercises and exercise definitions based on user ID in claims
        private void GetUserExercises()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _userExerciseDefinitions = _exerciseDefinitionRepo.AsQueryable().Where(e => e.User == userId).ToList();
            _userExercises = new List<Exercise>();
            _userExerciseDefinitions.ForEach(def =>
            {
                IQueryable<Exercise> defExercises = _exerciseRepo.AsQueryable().Where(e => e.Definition == def.Id);
                _userExercises.AddRange(defExercises.ToList());
            });
        }

        private List<AnalyticsItem> GetExerciseFrequency()
        {
            // Increment muscle group by occurance
            List<AnalyticsItem> exerciseFrequency = new List<AnalyticsItem>();
            _userExerciseDefinitions.ForEach(e =>
            {
                exerciseFrequency.Add(new AnalyticsItem()
                {
                    Label = e.Title,
                    Count = e.History == null ? 0 : e.History.Count
                });
            });
            return exerciseFrequency;
        }

        private AnalyticsItem GetMostFrequentExercise(List<AnalyticsItem> exerciseFrequency)
        {
            AnalyticsItem mostFrequentExercise = new AnalyticsItem();
            exerciseFrequency.ForEach(e =>
           {
               if (e.Count > mostFrequentExercise.Count)
               {
                   mostFrequentExercise.Label = e.Label;
                   mostFrequentExercise.Count = e.Count;
               }
           });
            return mostFrequentExercise;
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
                    ExerciseDefiniton def = _userExerciseDefinitions.Find(d => d.Id == defId);
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
                ExerciseDefiniton def = _userExerciseDefinitions.Find(d => d.Id == progressItem.Value.Id);
                Exercise firstExercise = _userExercises.Find(ex => ex.Id == def.History?.FirstOrDefault());

                double progressPercent = 0.0;
                if (firstExercise != null)
                {
                    double initialNetValue = ExerciseUtilities.GetNetExerciseValue(firstExercise);

                    // Progress percent = average net value - initial net value
                    progressPercent = (progressItem.Value.Total / progressItem.Value.NumberOfSessions) - initialNetValue;
                }


                exerciseProgress.Add(new AnalyticsItem
                {
                    Label = progressItem.Value.Title,
                    Count = progressPercent,
                });
            }

            return exerciseProgress;
        }

        private List<AnalyticsItem> GetMuscleGroupFrequency()
        {
            // Increment muscle group by occurance
            Dictionary<string, int> muscleGroupFrequency = new Dictionary<string, int>();
            _userExercises.ForEach(e =>
            {
                ExerciseDefiniton exerciseDefiniton = _userExerciseDefinitions.Find(def => def.Id == e.Definition);
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
                    Label = muscleGroup.Key,
                    Count = muscleGroup.Value
                });
            }
            return muscleGroupFrequencyList;
        }

        private AnalyticsItem GetMostFrequentMuscleGroup(List<AnalyticsItem> muscleGroupFrequency)
        {
            AnalyticsItem mostFrequentMuscleGroup = new AnalyticsItem();
            muscleGroupFrequency.ForEach(m =>
            {
                if (m.Count > mostFrequentMuscleGroup.Count)
                {
                    mostFrequentMuscleGroup.Label = m.Label;
                    mostFrequentMuscleGroup.Count = m.Count;
                }
            });
            return mostFrequentMuscleGroup;
        }
    }
}