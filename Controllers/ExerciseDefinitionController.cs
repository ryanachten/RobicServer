using System.Collections.Generic;
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
        private readonly IExerciseDefinitionRepository _exerciseDefinitionRepo;
        private readonly IExerciseRepository _exerciseRepo;
        private readonly IMapper _mapper;

        public ExerciseDefinitionController(
            IExerciseDefinitionRepository exerciseDefinitionRepo,
            IExerciseRepository exerciseRepo,
            IMongoRepository<Exercise> exerciseContext,
            IMongoRepository<ExerciseDefiniton> exerciseDefintionContext,
            IMongoRepository<User> userContext,
            IMapper mapper
        )
        {
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
            _exerciseRepo = exerciseRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public List<ExerciseDefinitionForListDto> GetDefinition()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var definitons = _exerciseDefinitionRepo.GetUserDefinitions(userId);
            var definitionsForReturn = _mapper.Map<List<ExerciseDefinitionForListDto>>(definitons);

            return definitionsForReturn;
        }

        [HttpGet("{id:length(24)}", Name = "GetExerciseDefinition")]
        public async Task<IActionResult> GetExeciseDefinition(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefinitionRepo.GetExerciseDefinition(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            exercise.PersonalBest = _exerciseRepo.GetPersonalBest(id);

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefinition(ExerciseDefiniton exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefinitionRepo.CreateDefinition(userId, exercise);

            return CreatedAtRoute("GetExerciseDefinition", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ExerciseDefiniton updatedExercise)
        {
            ExerciseDefiniton exercise = await _exerciseDefinitionRepo.GetExerciseDefinition(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            await _exerciseDefinitionRepo.UpdateDefinition(exercise, updatedExercise);

            return Ok(exercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefinitionRepo.GetExerciseDefinition(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefinitionRepo.DeleteDefinition(userId, id);

            return NoContent();
        }
    }
}