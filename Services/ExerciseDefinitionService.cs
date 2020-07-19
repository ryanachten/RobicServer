using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class ExerciseDefinitionService
    {
        private readonly IMongoCollection<ExerciseDefiniton> _exerciseDefinitions;

        public ExerciseDefinitionService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _exerciseDefinitions = database.GetCollection<ExerciseDefiniton>(settings.ExerciseDefinitionCollectionName);
        }

        public async Task<List<ExerciseDefiniton>> Get()
        {
            var exerciseDefinitions = await _exerciseDefinitions.Find<ExerciseDefiniton>(exercise => true).ToListAsync();
            return exerciseDefinitions;
        }
    }
}