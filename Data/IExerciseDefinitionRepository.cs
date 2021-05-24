using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public interface IExerciseDefinitionRepository
    {
        IEnumerable<ExerciseDefinition> GetUserDefinitions(string userId);
        Task<ExerciseDefinition> GetExerciseDefinition(string id);
        Task CreateDefinition(string userId, ExerciseDefinition definition);
        Task UpdateDefinition(ExerciseDefinition existingDefinition, ExerciseDefinition updatedDefinition);
        Task DeleteDefinition(string userId, string id);
        Task<bool> IsUsersDefinition(string userId, string definitionId);
    }
}