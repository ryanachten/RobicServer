using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public interface IExerciseRepository
    {
        List<Exercise> GetDefinitionExercises(string definitionId);
    }
}