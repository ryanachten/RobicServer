namespace RobicServer.Services
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string ExerciseDefinitionCollectionName { get; set; }

    }
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }

        // TODO: deprecate the following after generalisation
        string ExerciseDefinitionCollectionName { get; set; }
    }
}