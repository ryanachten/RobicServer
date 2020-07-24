using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Data
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User> Get(string id);
    }
}