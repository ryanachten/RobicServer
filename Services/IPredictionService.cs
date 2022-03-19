using RobicServer.Models;
using System.Threading.Tasks;

namespace RobicServer.Services
{
    public interface IPredictionService
    {
        public Task<PredictedResults> ForecastNetValue(string definitionId);
        public Task RegressionNetValue(string definitionId);

    }
}
