using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
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
                MuscleGroupFrequency = muscleGroupFrequency,
                MostFrequentMuscleGroup = this.GetMostFrequentMuscleGroup(muscleGroupFrequency)
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
                var defExercises = _exerciseRepo.AsQueryable().Where(e => e.Definition == def.Id);
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
                    label = e.Title,
                    count = e.History == null ? 0 : e.History.Count
                });
            });
            return exerciseFrequency;
        }

        private AnalyticsItem GetMostFrequentExercise(List<AnalyticsItem> exerciseFrequency)
        {
            AnalyticsItem mostFrequentExercise = new AnalyticsItem();
            exerciseFrequency.ForEach(e =>
           {
               if (e.count > mostFrequentExercise.count)
               {
                   mostFrequentExercise.label = e.label;
                   mostFrequentExercise.count = e.count;
               }
           });
            return mostFrequentExercise;
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
                    label = muscleGroup.Key,
                    count = muscleGroup.Value
                });
            }
            return muscleGroupFrequencyList;
        }

        private AnalyticsItem GetMostFrequentMuscleGroup(List<AnalyticsItem> muscleGroupFrequency)
        {
            AnalyticsItem mostFrequentMuscleGroup = new AnalyticsItem();
            muscleGroupFrequency.ForEach(m =>
            {
                if (m.count > mostFrequentMuscleGroup.count)
                {
                    mostFrequentMuscleGroup.label = m.label;
                    mostFrequentMuscleGroup.count = m.count;
                }
            });
            return mostFrequentMuscleGroup;
        }
    }
}