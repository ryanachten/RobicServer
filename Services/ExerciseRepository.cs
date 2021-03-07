using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RobicServer.Helpers;
using RobicServer.Models;

namespace RobicServer.Services
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;

        public ExerciseRepository(IMongoRepository<Exercise> exerciseContext, IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext)
        {
            _exerciseContext = exerciseContext;
            _exerciseDefinitionContext = exerciseDefinitionContext;
        }

        public async Task<Exercise> CreateExercise(Exercise exercise, ExerciseDefiniton definition)
        {
            exercise.Date = DateTime.Now;

            await _exerciseContext.InsertOneAsync(exercise);

            Exercise latestExercise = await _exerciseContext.FindByIdAsync(definition.History.LastOrDefault());

            // Add exercise to definition history
            definition.History.Add(exercise.Id);

            // Update definition aggregate fields
            definition.LastSession = exercise;
            if (latestExercise != null)
                definition.LastImprovement = GetLatestExerciseImprovement(exercise, latestExercise);

            await _exerciseDefinitionContext.ReplaceOneAsync(definition);

            return exercise;
        }

        public async Task DeleteExercise(string id, ExerciseDefiniton definiton)
        {
            await _exerciseContext.DeleteByIdAsync(id);

            // Remove exercise from definition history
            definiton.History.Remove(id);
            await _exerciseDefinitionContext.ReplaceOneAsync(definiton);
        }

        public List<Exercise> GetDefinitionExercises(string definitionId)
        {
            // Filter exercises to only those  associated with the user's definitions
            var exercises = _exerciseContext.AsQueryable()
                .Where(exercise => exercise.Definition == definitionId).ToList();
            return exercises;
        }

        public async Task<Exercise> GetExerciseById(string id)
        {
            return await _exerciseContext.FindByIdAsync(id);
        }

        public async Task UpdateExercise(Exercise updatedExercise)
        {
            await _exerciseContext.ReplaceOneAsync(updatedExercise);
        }

        private double GetLatestExerciseImprovement(Exercise newExercise, Exercise lastExercise)
        {
            var newNetValue = ExerciseUtilities.GetNetExerciseValue(newExercise);
            var lastNetValue = ExerciseUtilities.GetNetExerciseValue(lastExercise);

            var min = Math.Min(newNetValue, lastNetValue);
            var max = Math.Max(newNetValue, lastNetValue);
            var delta = max - min;

            // Get a percentage of deviation based on average net value
            var improvement = (delta / max) * 100;

            // If most recent value is less than average, this is a negative correlation
            if (lastNetValue > newNetValue)
                improvement *= -1;

            return Math.Round(improvement);
        }
    }
}