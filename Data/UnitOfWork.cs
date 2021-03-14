using RobicServer.Interfaces;
using RobicServer.Models;

namespace RobicServer.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoRepository<User> _userContext;
        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;

        public UnitOfWork(IMongoRepository<User> userContext, IMongoRepository<Exercise> exerciseContext, IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext)
        {
            _exerciseDefinitionContext = exerciseDefinitionContext;
            _userContext = userContext;
            _exerciseContext = exerciseContext;
        }
        public IAuthRepository AuthRepo => new AuthRepository(_userContext);

        public IExerciseRepository ExerciseRepo => new ExerciseRepository(_exerciseContext, _exerciseDefinitionContext);

        public IExerciseDefinitionRepository ExerciseDefinitionRepo => new ExerciseDefinitionRepository(_exerciseDefinitionContext, _exerciseContext, _userContext);

        public IUserRepository UserRepo => new UserRepository(_userContext, _exerciseContext, _exerciseDefinitionContext);
    }
}