using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class ExerciseService
    {
        private readonly IMongoCollection<Exercise> _exercises;

        public ExerciseService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _exercises = database.GetCollection<Exercise>(settings.ExerciseCollectionName);
        }

        public async Task<List<Exercise>> Get()
        {
            var exercises = await _exercises.Find<Exercise>(exercise => true).ToListAsync();
            return exercises;
        }

        public Exercise Get(string id) =>
            _exercises.Find<Exercise>(exercise => exercise.Id == id).FirstOrDefault();

        public Exercise Create(Exercise exercise)
        {
            _exercises.InsertOne(exercise);
            return exercise;
        }

        public void Update(string id, Exercise updatedExercise)
        {
            _exercises.ReplaceOne((Exercise exercise) => exercise.Id == id, updatedExercise);
        }

        public void Delete(Exercise exercise)
        {
            _exercises.DeleteOne(exercise.Id);
        }

        public void Delete(string id)
        {
            _exercises.DeleteOne(id);
        }
    }
}