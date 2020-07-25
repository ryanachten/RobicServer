using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using RobicServer.Models.DTOs;

namespace RobicServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userDetails)
        {
            User user = _mapper.Map<User>(userDetails);
            var createdUser = await _repo.Register(user, userDetails.Password);

            return Ok(createdUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userDetails)
        {
            User user = await _repo.Login(userDetails.Email.ToLower(), userDetails.Password);
            if (user == null)
                return Unauthorized();

            // TODO: add claims and token handling

            return Ok(user);
        }
    }
}