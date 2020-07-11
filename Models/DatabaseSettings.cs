namespace RobicServer.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string ExerciseCollectionName { get; set; }
        public string ExerciseDefinitionCollectionName { get; set; }
        public string SessionCollectionName { get; set; }
        public string UserCollectionName { get; set; }

    }
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string ExerciseCollectionName { get; set; }
        string ExerciseDefinitionCollectionName { get; set; }
        string SessionCollectionName { get; set; }
        string UserCollectionName { get; set; }
    }
}