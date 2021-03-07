using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class ExerciseDefinitionRepository : IExerciseDefinitionRepository
    {
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;

        public ExerciseDefinitionRepository(IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext)
        {
            _exerciseDefinitionContext = exerciseDefinitionContext;
        }

        public async Task<ExerciseDefiniton> GetExerciseDefinition(string id)
        {
            return await _exerciseDefinitionContext.FindByIdAsync(id);
        }

        public IEnumerable<ExerciseDefiniton> GetUserDefinitions(string userId)
        {
            return _exerciseDefinitionContext.FilterBy(defintion => defintion.User == userId);
        }

        public async Task<bool> IsUsersDefinition(string userId, string definitionId)
        {
            ExerciseDefiniton definiton = await _exerciseDefinitionContext.FindByIdAsync(definitionId);
            return definiton != null && definiton.User == userId;
        }
    }
}