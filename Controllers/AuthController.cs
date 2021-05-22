using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;
using RobicServer.Models.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _repo;

        public AuthController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _config = config;
            _mapper = mapper;
            _repo = unitOfWork.AuthRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userDetails)
        {
            if (await _repo.UserExists(userDetails.Email))
                return BadRequest($"Email {userDetails.Email} already registered to Robic");

            User user = _mapper.Map<User>(userDetails);
            var createdUser = await _repo.Register(user, userDetails.Password);

            var userForReturn = _mapper.Map<UserForDetailDto>(createdUser);

            return CreatedAtRoute("GetUser", new { controller = "User", id = createdUser.Id }, userForReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userLoginDetails)
        {
            User user = await _repo.Login(userLoginDetails.Email.ToLower(), userLoginDetails.Password);
            if (user == null)
                return Unauthorized();

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TokenKey"))
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddYears(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userDetails = _mapper.Map<UserForDetailDto>(user);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                userDetails
            });
        }
    }
}