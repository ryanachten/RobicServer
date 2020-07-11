namespace RobicServer.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ExerciseCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IDatabaseSettings
    {
        string ExerciseCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}