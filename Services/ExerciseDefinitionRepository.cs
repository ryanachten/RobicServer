using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class ExerciseDefinitionRepository : IExerciseDefinitionRepository
    {
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;
        private readonly IMongoRepository<User> _userContext;

        public ExerciseDefinitionRepository(IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext, IMongoRepository<User> userContext)
        {
            _exerciseDefinitionContext = exerciseDefinitionContext;
            _userContext = userContext;
        }

        public async Task CreateDefinition(string userId, ExerciseDefiniton definition)
        {
            await _exerciseDefinitionContext.InsertOneAsync(definition);

            // Update user's exercises with new exercise
            User user = await _userContext.FindByIdAsync(userId);
            user.Exercises.Add(definition.Id);
            await _userContext.ReplaceOneAsync(user);
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

        public async Task UpdateDefinition(ExerciseDefiniton existingDefinition, ExerciseDefiniton updatedDefinition)
        {
            existingDefinition.Title = updatedDefinition.Title;
            existingDefinition.Unit = updatedDefinition.Unit;
            existingDefinition.PrimaryMuscleGroup = updatedDefinition.PrimaryMuscleGroup;

            await _exerciseDefinitionContext.ReplaceOneAsync(existingDefinition);
        }
    }
}