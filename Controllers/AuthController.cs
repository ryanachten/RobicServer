using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;

namespace RobicServer.Controllers
{

    // TODO: replace with proper DTOs
    public struct UserDetailDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDetailDto userDetails)
        {
            // TODO: check for existence of email in DB via UserExists method
            // TODO: user AutoMapper to concert DTO's to objects
            User userToRegister = new User();
            userToRegister.Email = userDetails.Email;
            var createdUser = await _repo.Register(userToRegister, userDetails.Password);

            return Ok(createdUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDetailDto userDetails)
        {
            User user = await _repo.Login(userDetails.Email.ToLower(), userDetails.Password);
            if (user == null)
                return Unauthorized();

            // TODO: add claims and token handling

            return Ok(user);
        }
    }
}