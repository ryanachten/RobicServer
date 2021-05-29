using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public interface IAnalyticsRepository
    {
        Task<Analytics> GetUserAnalytics(string userId);
    }
}