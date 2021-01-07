using System;
using System.Collections.Generic;
using System.Linq;
using RobicServer.Models;
using RobicServer.Services;

namespace RobicServer.Helpers
{
    public class ExerciseUtilities
    {
        private readonly IQueryable<Exercise> _exercises;

        public ExerciseUtilities(IQueryable<Exercise> exercises)
        {
            _exercises = exercises;
        }

        public static double GetNetExerciseValue(Exercise exercise)
        {
            double total = 0.0;
            foreach (Set set in exercise.Sets)
            {
                if (set.Reps.HasValue && set.Value.HasValue)
                    total += (double)set.Reps * (double)set.Value;
            }
            return total;
        }

        public double? GetLatestExerciseImprovement(string definitionId)
        {
            var mostRecentExercise = _exercises.FirstOrDefault();
            if (mostRecentExercise == null || !mostRecentExercise.NetValue.HasValue)
            {
                return null;
            }

            // Get net values for all exercises
            double totalNetValues = 0.0;
            foreach (var e in _exercises)
            {
                if (e.NetValue.HasValue)
                {
                    totalNetValues += (double)e.NetValue;
                }
            }

            // Get average exercise net value
            var averageNetValue = totalNetValues / _exercises.Count();
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
            if (_exercises == null)
            {
                return null;
            }
            Exercise? exerciseWithHighestNetValue = null;
            double highestAvgValue = 0;
            int highestReps = 0;
            int highestSets = 0;
            var history = new List<PersonalBestHistory>();

            foreach (var e in _exercises)
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
            }

            return new PersonalBest
            {
                TopNetExercise = exerciseWithHighestNetValue,
                TopAvgValue = highestAvgValue,
                TopSets = highestSets,
                TopReps = highestReps,
                History = history
            };
        }

        private PersonalBestHistory GetPersonalBestHistory(Exercise exercise)
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
            var record = new PersonalBestHistory()
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