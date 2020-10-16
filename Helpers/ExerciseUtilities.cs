using System;
using System.Linq;
using RobicServer.Models;
using RobicServer.Models.DTOs;
using RobicServer.Services;

namespace RobicServer.Helpers
{
    public class ExerciseUtilities
    {
        private readonly IMongoRepository<Exercise> _exerciseRepo;

        public ExerciseUtilities(IMongoRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }
        public DateTime? GetLatestExerciseDate(string definitionId)
        {

            var latestExercise = _exerciseRepo.AsQueryable().Where(exercise => exercise.Definition == definitionId).OrderByDescending(d => d.Date).FirstOrDefault();
            if (latestExercise != null)
            {
                return latestExercise.Date;
            }
            return null;
        }

        public double? GetLatestExerciseImprovement(string definitionId)
        {

            var exercises = _exerciseRepo.AsQueryable().Where(exercise => exercise.Definition == definitionId);
            var mostRecentExercise = exercises.OrderByDescending(d => d.Date).FirstOrDefault();
            if (mostRecentExercise == null || !mostRecentExercise.NetValue.HasValue)
            {
                return null;
            }

            // Get net values for all exercises
            double totalNetValues = 0.0;
            exercises.ToList().ForEach(e =>
            {
                if (e.NetValue.HasValue)
                {
                    totalNetValues += (double)e.NetValue;
                }
            });

            // Get average exercise net value
            var averageNetValue = totalNetValues / exercises.Count();
            var mostRecentNetValue = mostRecentExercise.NetValue;
            var min = Math.Min(averageNetValue, (double)mostRecentNetValue);
            var max = Math.Max(averageNetValue, (double)mostRecentNetValue);

            // Get a percentage of deviation based on average net value
            var improvement = (min / max) * 100;

            // If most recent value is less than average, this is a negative correlation
            if (averageNetValue < mostRecentNetValue)
                improvement *= -1;
            return Math.Round(improvement);
        }
    }
}