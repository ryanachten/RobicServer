using System.Collections.Generic;
using RobicServer.Models;

namespace RobicServer.Interfaces
{
    public interface IAnalyticsRepository
    {
        void GetUserExercises(string userId, out List<ExerciseDefiniton> userExerciseDefinitions, out List<Exercise> userExercises);
        List<AnalyticsItem> GetExerciseFrequency(List<ExerciseDefiniton> exerciseDefinitions);
        List<AnalyticsItem> GetExerciseProgress(List<Exercise> exercises, List<ExerciseDefiniton> exerciseDefinitions);
        List<AnalyticsItem> GetMuscleGroupFrequency(List<Exercise> exercises, List<ExerciseDefiniton> exerciseDefinitions);
    }
}