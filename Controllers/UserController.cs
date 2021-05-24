using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
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
        private readonly IUserRepository _userRepo;

        public UserController(
            IUnitOfWork unitOfWork,
            IUserRepository userRepo,
            IMongoRepository<Exercise> exerciseRepo,
            IMongoRepository<ExerciseDefinition> exerciseDefinitionRepo,
            IMapper mapper
        )
        {
            _mapper = mapper;
            _userRepo = unitOfWork.UserRepo;
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public async Task<IActionResult> Get(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId != id)
                return Unauthorized();

            User user = await _userRepo.GetUser(id);

            if (user == null)
                return NotFound();

            var userForReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userForReturn);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId != id)
                return Unauthorized();

            User user = await _userRepo.GetUser(id);
            if (user == null)
                return NotFound();

            await _userRepo.DeleteUser(user);

            return NoContent();
        }
    }
}