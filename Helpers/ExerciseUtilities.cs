using System;
using System.Collections.Generic;
using System.Linq;
using RobicServer.Models;
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
#nullable enable
        public Exercise? GetLatestExercise(string definitionId)
        {

            var latestExercise = _exerciseRepo.AsQueryable().Where(exercise => exercise.Definition == definitionId).OrderByDescending(d => d.Date).FirstOrDefault();
            if (latestExercise != null)
            {
                return latestExercise;
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
#nullable enable
        public PersonalBest? GetPersonalBest(string definitionId)
        {
            var exercises = _exerciseRepo.AsQueryable().Where(exercise => exercise.Definition == definitionId);
            if (exercises == null)
            {
                return null;
            }
            Exercise? exerciseWithHighestNetValue = null;
            double highestAvgValue = 0;
            int highestReps = 0;
            int highestSets = 0;
            var history = new List<ExerciseRecords>();

            exercises.ToList().ForEach(e =>
            {
                if (e.NetValue.HasValue && (exerciseWithHighestNetValue == null || exerciseWithHighestNetValue.NetValue < e.NetValue))
                    exerciseWithHighestNetValue = e;

                if (e.Sets.Count > highestSets)
                    highestSets = e.Sets.Count;

                double totalValue = 0;
                e.Sets.ToList().ForEach(s =>
                {
                    if (s.Reps.HasValue && s.Reps > highestReps)
                    {
                        highestReps = (int)s.Reps;
                    }

                    if (s.Value.HasValue)
                    {
                        totalValue += (int)s.Value;
                    }
                });
                double avgValue = totalValue / e.Sets.Count();
                if (avgValue > highestAvgValue)
                    highestAvgValue = avgValue;

                history.Add(this.GetPersonalBestHistory(e));

            });
            return new PersonalBest
            {
                TopNetExercise = exerciseWithHighestNetValue,
                TopAvgValue = highestAvgValue,
                TopSets = highestSets,
                TopReps = highestReps,
                History = history
            };
        }

        private ExerciseRecords GetPersonalBestHistory(Exercise exercise)
        {
            var totalReps = 0.0;
            var totalValue = 0.0;
            exercise.Sets.ToList().ForEach(s =>
            {
                if (s.Reps.HasValue)
                {
                    totalReps += (double)s.Reps;
                }
                if (s.Value.HasValue)
                {
                    totalValue += (double)s.Value;
                }
            });
            var record = new ExerciseRecords()
            {
                Date = exercise.Date,
                NetValue = exercise.NetValue,
                Sets = exercise.Sets.Count,
                TimeTaken = exercise.TimeTaken,
                AvgReps = totalReps / exercise.Sets.Count,
                AvgValue = totalValue / exercise.Sets.Count,
            };
            return record;
        }
    }
}