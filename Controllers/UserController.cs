using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using AutoMapper;
using RobicServer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Collections.Generic;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMongoRepository<User> _userRepo;
        private readonly IMongoRepository<Exercise> _exerciseRepo;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionRepo;

        public UserController(
            IMongoRepository<User> userRepo,
            IMongoRepository<Exercise> exerciseRepo,
            IMongoRepository<ExerciseDefiniton> exerciseDefinitionRepo,
            IMapper mapper
        )
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _exerciseRepo = exerciseRepo;
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
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

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId != id)
                return Unauthorized();

            User user = await _userRepo.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Clean up associated user data
            foreach (var definitionId in user.Exercises)
            {
                var definiton = await _exerciseDefinitionRepo.FindByIdAsync(definitionId);
                foreach (var exerciseId in definiton.History)
                {
                    // Remove all associated exercise sesssions
                    await _exerciseRepo.DeleteByIdAsync(exerciseId);
                }
                // Remove all associated exercise definitions
                await _exerciseDefinitionRepo.DeleteByIdAsync(definitionId);
            }

            await _userRepo.DeleteByIdAsync(user.Id);
            return NoContent();
        }
    }
}