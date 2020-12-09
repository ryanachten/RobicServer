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

        public AnalyticsController(IMongoRepository<Exercise> exerciseRepo, IMongoRepository<ExerciseDefiniton> exerciseDefinitionRepo)
        {
            _exerciseRepo = exerciseRepo;
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
        }

        [HttpGet]
        public Analytics Get()
        {
            List<AnalyticsItem> muscleGroupFrequency = this.GetMuscleGroupFrequency();
            Analytics analytics = new Analytics()
            {
                // MostFrequentExercise = this.GetMostFrequentExercise(),
                // MostFrequentMuscleGroup = this.GetMostFrequentMuscleGroup(muscleGroupFrequency),
                MuscleGroupFrequency = muscleGroupFrequency
            };

            return analytics;
        }

        private AnalyticsItem GetMostFrequentExercise()
        {
            AnalyticsItem mostFrequentExercise = new AnalyticsItem();
            _exerciseDefinitionRepo.AsQueryable().ToList().ForEach(e =>
           {
               if (e.History.Count < mostFrequentExercise.count)
               {
                   mostFrequentExercise.label = e.Title;
                   mostFrequentExercise.count = e.History.Count;
               }
           });
            return mostFrequentExercise;
        }

        private List<AnalyticsItem> GetMuscleGroupFrequency()
        {
            // Increment muscle group by occurance
            Dictionary<string, int> muscleGroupFrequency = new Dictionary<string, int>();
            foreach (var e in _exerciseRepo.AsQueryable())
            {
                ExerciseDefiniton exerciseDefiniton = _exerciseDefinitionRepo.AsQueryable().FirstOrDefault(def => def.Id == e.Definition);
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
            }
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
                if (m.count < mostFrequentMuscleGroup.count)
                {
                    mostFrequentMuscleGroup.label = m.label;
                    mostFrequentMuscleGroup.count = m.count;
                }
            });
            return mostFrequentMuscleGroup;
        }
    }
}