using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UserCollectionName);
        }

        public Task<User> Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
    }
}