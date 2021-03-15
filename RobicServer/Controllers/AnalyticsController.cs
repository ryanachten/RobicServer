using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Interfaces;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RobicServer.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository _analyticsRepo;

        public AnalyticsController(IUnitOfWork unitOfWork)
        {
            _analyticsRepo = unitOfWork.AnalyticsRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Exercise> userExercises;
            List<ExerciseDefiniton> userExerciseDefinitions;
            _analyticsRepo.GetUserExercises(userId, out userExerciseDefinitions, out userExercises);

            List<AnalyticsItem> muscleGroupFrequency = _analyticsRepo.GetMuscleGroupFrequency(userExercises, userExerciseDefinitions);
            List<AnalyticsItem> exerciseFrequency = _analyticsRepo.GetExerciseFrequency(userExerciseDefinitions);

            Analytics analytics = new Analytics()
            {
                ExerciseFrequency = exerciseFrequency,
                MostFrequentExercise = exerciseFrequency.FirstOrDefault(),
                ExerciseProgress = _analyticsRepo.GetExerciseProgress(userExercises, userExerciseDefinitions),
                MuscleGroupFrequency = muscleGroupFrequency,
                MostFrequentMuscleGroup = muscleGroupFrequency.FirstOrDefault(),
            };

            return Ok(analytics);
        }
    }
}