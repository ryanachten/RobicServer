using RobicServer.Models;
using System.Threading.Tasks;

namespace RobicServer.Services
{
    public interface IPredictionService
    {
        public Task<PredictedResults> PredictNetValue(string definitionId);
    }
}
