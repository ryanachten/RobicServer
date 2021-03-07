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

        public List<Exercise> GetUserExercises(string userId)
        {
            // Filter exercises to only those  associated with the user's definitions
            var exerciseDefinitionIds = _exerciseDefinitionContext.AsQueryable()
                .Where(exercise => exercise.User == userId)
                .Select(exerciseDefinitions => exerciseDefinitions.Id).ToArray();
            var exercises = _exerciseContext.AsQueryable().Where(exercise => exerciseDefinitionIds.Contains(exercise.Definition)).ToList();
            return exercises;
        }
    }
}