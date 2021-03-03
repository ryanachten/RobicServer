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

        public static double? GetLatestExerciseImprovement(Exercise newExercise, Exercise lastExercise)
        {
            var newNetValue = ExerciseUtilities.GetNetExerciseValue(newExercise);
            var lastNetValue = ExerciseUtilities.GetNetExerciseValue(lastExercise);

            var min = Math.Min(newNetValue, lastNetValue);
            var max = Math.Max(newNetValue, lastNetValue);
            var delta = max - min;

            // Get a percentage of deviation based on average net value
            var improvement = (delta / max) * 100;

            // If most recent value is less than average, this is a negative correlation
            if (lastNetValue > newNetValue)
                improvement *= -1;

            return Math.Round(improvement);
        }
#nullable enable
        public PersonalBest? GetPersonalBest(string definitionId)
        {
            if (_exercises == null || _exercises.Count() == 0)
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
                foreach (var s in e.Sets)
                {
                    if (s.Reps.HasValue && s.Reps > highestReps)
                    {
                        highestReps = (int)s.Reps;
                    }

                    if (s.Value.HasValue)
                    {
                        totalValue += (int)s.Value;
                    }
                }
                double avgValue = totalValue / e.Sets.Count();
                if (avgValue > highestAvgValue)
                    highestAvgValue = avgValue;

                if (e.Sets.Count > 0)
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
            foreach (var s in exercise.Sets)
            {
                if (s.Reps.HasValue)
                {
                    totalReps += (double)s.Reps;
                }
                if (s.Value.HasValue)
                {
                    totalValue += (double)s.Value;
                }
            }
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