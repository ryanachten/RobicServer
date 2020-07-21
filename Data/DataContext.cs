using MongoDB.Driver;
using RobicServer.Models;

namespace RobicServer.Data
{
    public class DataContext
    {
        private readonly IDatabaseSettings _settings;
        private readonly IMongoDatabase _database;

        public DataContext(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _settings = settings;
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>(_settings.UserCollectionName); }
        }

    }
}