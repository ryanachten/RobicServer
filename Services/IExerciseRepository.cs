using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public interface IExerciseRepository
    {
        List<Exercise> GetDefinitionExercises(string definitionId);
        Task<Exercise> GetExerciseById(string id);

        // TODO: should be in exercise def repo
        Task<bool> IsUsersDefinition(string userId, string definitionId);
    }
}