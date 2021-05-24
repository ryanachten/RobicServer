using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public interface IExerciseRepository
    {
        List<Exercise> GetDefinitionExercises(string definitionId);
        Task<Exercise> GetExerciseById(string id);
        Task<Exercise> CreateExercise(Exercise exercise, ExerciseDefinition definiton);
        Task UpdateExercise(Exercise exercise);
        Task DeleteExercise(string id, ExerciseDefinition definiton);
        PersonalBest GetPersonalBest(string defintionId);
    }
}