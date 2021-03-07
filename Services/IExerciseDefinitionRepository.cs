using System.Collections.Generic;
using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Services
{
    public interface IExerciseDefinitionRepository
    {
        IEnumerable<ExerciseDefiniton> GetUserDefinitions(string userId);
    }
}