using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Interfaces
{
    public interface IExerciseDefinitionRepository
    {
        IEnumerable<ExerciseDefiniton> GetUserDefinitions(string userId);
        Task<ExerciseDefiniton> GetExerciseDefinition(string id);
        Task CreateDefinition(string userId, ExerciseDefiniton definition);
        Task UpdateDefinition(ExerciseDefiniton existingDefinition, ExerciseDefiniton updatedDefinition);
        Task DeleteDefinition(string userId, string id);
        Task<bool> IsUsersDefinition(string userId, string definitionId);
    }
}