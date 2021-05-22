namespace RobicServer.Data
{
    public interface IUnitOfWork
    {
        IAuthRepository AuthRepo { get; }
        IExerciseRepository ExerciseRepo { get; }
        IExerciseDefinitionRepository ExerciseDefinitionRepo { get; }
        IUserRepository UserRepo { get; }
    }
}