using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public interface IExerciseDefinitionRepository
    {
        IEnumerable<ExerciseDefinition> GetUserDefinitions(string userId);
        Task<ExerciseDefinition> GetExerciseDefinition(string id);
        Task<ExerciseDefinition> CreateDefinition(string userId, ExerciseDefinition definition);
        Task<ExerciseDefinition> UpdateDefinition(ExerciseDefinition existingDefinition, ExerciseDefinition updatedDefinition);
        Task DeleteDefinition(ExerciseDefinition definition);
        Task<bool> IsUsersDefinition(string userId, string definitionId);
    }
}