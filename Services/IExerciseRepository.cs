using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public interface IExerciseRepository
    {
        List<Exercise> GetDefinitionExercises(string definitionId);
        Task<Exercise> GetExerciseById(string id);
        Task<Exercise> CreateExercise(Exercise exercise, ExerciseDefiniton definiton);
        Task UpdateExercise(Exercise exercise);
        Task DeleteExercise(string id, ExerciseDefiniton definiton);

        // TODO: should be in exercise def repo
        Task<bool> IsUsersDefinition(string userId, string definitionId);
    }
}