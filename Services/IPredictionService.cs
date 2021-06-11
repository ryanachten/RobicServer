using System.Threading.Tasks;

namespace RobicServer.Services
{
    public interface IPredictionService
    {
        public Task<object> PredictNetValue(string definitionId);
    }
}
