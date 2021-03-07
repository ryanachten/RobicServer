using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using RobicServer.Helpers;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionContext;

        public ExerciseController(IExerciseRepository exerciseRepository, IMongoRepository<Exercise> exerciseContext, IMongoRepository<ExerciseDefiniton> exerciseDefinitionContext)
        {
            _exerciseRepository = exerciseRepository;
            _exerciseContext = exerciseContext;
            _exerciseDefinitionContext = exerciseDefinitionContext;
        }

        [HttpGet]
        public IActionResult GetDefinitionExercises([FromQuery(Name = "definition")] string definitionId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definition = _exerciseDefinitionContext.FindById(definitionId);
            if (definition == null)
                return NotFound();

            if (definition.User != userId)
                return Unauthorized();

            var exercises = _exerciseRepository.GetDefinitionExercises(definition.Id);
            return Ok(exercises);
        }

        [HttpGet("{id:length(24)}", Name = "GetExercise")]
        public async Task<IActionResult> GetExerciseById(string id)
        {
            Exercise exercise = await _exerciseRepository.GetExerciseById(id);
            if (exercise == null)
                return NotFound();

            string userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isUserExercise = await _exerciseRepository.IsUsersDefinition(userID, exercise.Definition);
            if (!isUserExercise)
                return Unauthorized();

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExercise(Exercise exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definition = await _exerciseDefinitionContext.FindByIdAsync(exercise.Definition);

            if (definition == null || definition.User != userId)
                return Unauthorized();

            var createdExercise = await _exerciseRepository.CreateExercise(exercise, definition);

            return CreatedAtRoute("GetExercise", new { id = createdExercise.Id }, createdExercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Exercise updatedExercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isUserExercise = await _exerciseRepository.IsUsersDefinition(userId, updatedExercise.Definition);
            if (!isUserExercise)
                return Unauthorized();

            Exercise exercise = await _exerciseContext.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            await _exerciseContext.ReplaceOneAsync(updatedExercise);
            return Ok(updatedExercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            Exercise exercise = await _exerciseContext.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definiton = await _exerciseDefinitionContext.FindByIdAsync(exercise.Definition);

            if (definiton == null || definiton.User != userId)
                return Unauthorized();

            await _exerciseContext.DeleteByIdAsync(id);

            // Remove exercise from definition history
            definiton.History.Remove(id);
            await _exerciseDefinitionContext.ReplaceOneAsync(definiton);

            return NoContent();
        }
    }
}