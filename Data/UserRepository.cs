using System.Threading.Tasks;
using MongoDB.Driver;
using RobicServer.Models;

namespace RobicServer.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public Task<User> Create(User user)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> Get(string id)
        {
            User user = await _context.Users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
            return user;
        }
    }
}