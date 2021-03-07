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
        public IEnumerable<ExerciseDefiniton> GetUserDefinitions(string userId)
        {
            return _exerciseDefinitionContext.FilterBy(defintion => defintion.User == userId);
        }
    }
}