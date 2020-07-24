using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMongoRepository<User> _userRepo;

        public UserController(IMongoRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("{id}")]
        public async Task<User> Get(string id) => await _userRepo.FindByIdAsync(id);
    }
}