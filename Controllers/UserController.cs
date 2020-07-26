using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using AutoMapper;
using RobicServer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMongoRepository<User> _userRepo;

        public UserController(IMongoRepository<User> userRepo, IMapper mapper)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public async Task<IActionResult> Get(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId != id)
                return Unauthorized();

            User user = await _userRepo.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userForReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userForReturn);
        }
    }
}