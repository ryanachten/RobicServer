namespace RobicServer.Models
{
    public class ExerciseDatabaseSettings : IExerciseDatabaseSettings
    {
        public string ExerciseCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IExerciseDatabaseSettings
    {
        string ExerciseCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}