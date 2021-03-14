using System.Threading.Tasks;
using RobicServer.Models;

namespace RobicServer.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUser(string id);
        Task DeleteUser(User user);
    }
}