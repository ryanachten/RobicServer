using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public class ExerciseDefinitionRepository : IExerciseDefinitionRepository
    {
        private readonly IMongoRepository<ExerciseDefinition> _exerciseDefinitionContext;
        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<User> _userContext;

        public ExerciseDefinitionRepository(IMongoRepository<ExerciseDefinition> exerciseDefinitionContext, IMongoRepository<Exercise> exerciseContext, IMongoRepository<User> userContext)
        {
            _exerciseDefinitionContext = exerciseDefinitionContext;
            _exerciseContext = exerciseContext;
            _userContext = userContext;
        }

        public async Task<ExerciseDefinition> CreateDefinition(string userId, ExerciseDefinition definition)
        {
            await _exerciseDefinitionContext.InsertOneAsync(definition);

            // Update user's exercises with new exercise
            User user = await _userContext.FindByIdAsync(userId);
            user.Exercises.Add(definition.Id);
            await _userContext.ReplaceOneAsync(user);
            return definition;
        }

        public async Task DeleteDefinition(string userId, string id)
        {
            await _exerciseDefinitionContext.DeleteByIdAsync(id);

            // Remove definition from user exercises
            User user = await _userContext.FindByIdAsync(userId);
            user.Exercises.Remove(id);
            await _userContext.ReplaceOneAsync(user);

            // Remove exercises associated with definition
            await _exerciseContext.DeleteManyAsync(e => e.Definition == id);
        }

        public async Task<ExerciseDefinition> GetExerciseDefinition(string id)
        {
            return await _exerciseDefinitionContext.FindByIdAsync(id);
        }

        public IEnumerable<ExerciseDefinition> GetUserDefinitions(string userId)
        {
            return _exerciseDefinitionContext.FilterBy(defintion => defintion.User == userId);
        }

        public async Task<bool> IsUsersDefinition(string userId, string definitionId)
        {
            ExerciseDefinition definiton = await _exerciseDefinitionContext.FindByIdAsync(definitionId);
            return definiton != null && definiton.User == userId;
        }

        public async Task UpdateDefinition(ExerciseDefinition existingDefinition, ExerciseDefinition updatedDefinition)
        {
            existingDefinition.Title = updatedDefinition.Title;
            existingDefinition.Unit = updatedDefinition.Unit;
            existingDefinition.PrimaryMuscleGroup = updatedDefinition.PrimaryMuscleGroup;

            await _exerciseDefinitionContext.ReplaceOneAsync(existingDefinition);
        }
    }
}