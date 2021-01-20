using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using RobicServer.Models.DTOs;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseDefinitionController : ControllerBase
    {
        private readonly IMongoRepository<Exercise> _exerciseRepo;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefintionRepo;
        private readonly IMongoRepository<User> _userRepo;
        private readonly IMapper _mapper;

        public ExerciseDefinitionController(
            IMongoRepository<Exercise> exerciseRepo,
            IMongoRepository<ExerciseDefiniton> exerciseDefintionRepo,
            IMongoRepository<User> userRepo,
            IMapper mapper
        )
        {
            _exerciseRepo = exerciseRepo;
            _exerciseDefintionRepo = exerciseDefintionRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public List<ExerciseDefinitionForListDto> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var definitons = _exerciseDefintionRepo.FilterBy(defintion => defintion.User == userId);
            var definitionsForReturn = _mapper.Map<List<ExerciseDefinitionForListDto>>(definitons);

            return definitionsForReturn;
        }

        [HttpGet("{id:length(24)}", Name = "GetExerciseDefinition")]
        public async Task<IActionResult> Get(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseDefiniton exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefintionRepo.InsertOneAsync(exercise);

            // Update user's exercises with new exercise
            User user = await _userRepo.FindByIdAsync(userId);
            user.Exercises.Add(exercise.Id);
            await _userRepo.ReplaceOneAsync(user);

            return CreatedAtRoute("GetExerciseDefinition", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ExerciseDefiniton updatedExercise)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            exercise.Title = updatedExercise.Title;
            exercise.Unit = updatedExercise.Unit;
            exercise.PrimaryMuscleGroup = updatedExercise.PrimaryMuscleGroup;

            await _exerciseDefintionRepo.ReplaceOneAsync(exercise);
            return Ok(exercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefintionRepo.DeleteByIdAsync(id);

            // Remove definition from user exercises
            User user = await _userRepo.FindByIdAsync(userId);
            user.Exercises.Remove(exercise.Id);
            await _userRepo.ReplaceOneAsync(user);

            // Remove exercises associated with definition
            await _exerciseRepo.DeleteManyAsync(e => e.Definition == id);

            return NoContent();
        }
    }
}