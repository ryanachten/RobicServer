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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDetailDto userDetails)
        {
            User user = await _repo.Login(userDetails.Email.ToLower(), userDetails.Password);
            if (user == null)
                return Unauthorized();

            return Ok(user);
        }
    }
}