using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;

        public ExerciseRepository(IMongoRepository<Exercise> exerciseContext, IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext)
        {
            _exerciseContext = exerciseContext;
            _exerciseDefinitionContext = exerciseDefinitionContext;
        }

        public List<Exercise> GetDefinitionExercises(string definitionId)
        {
            // Filter exercises to only those  associated with the user's definitions
            var exercises = _exerciseContext.AsQueryable()
                .Where(exercise => exercise.Definition == definitionId).ToList();
            return exercises;
        }

        public async Task<Exercise> GetExerciseById(string id)
        {
            return await _exerciseContext.FindByIdAsync(id);
        }

        public async Task<bool> IsUsersDefinition(string userId, string definitionId)
        {
            ExerciseDefiniton definiton = await _exerciseDefinitionContext.FindByIdAsync(definitionId);
            return definiton != null && definiton.User == userId;
        }
    }
}