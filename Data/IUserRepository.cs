using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public interface IUserRepository
    {
        Task<User> Get(string id);
    }
}