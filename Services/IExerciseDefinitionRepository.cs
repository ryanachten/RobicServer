using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public interface IExerciseDefinitionRepository
    {
        IEnumerable<ExerciseDefiniton> GetUserDefinitions(string userId);
        Task<ExerciseDefiniton> GetExerciseDefinition(string id);
        Task CreateDefinition(string userId, ExerciseDefiniton definition);
        Task<bool> IsUsersDefinition(string userId, string definitionId);
    }
}