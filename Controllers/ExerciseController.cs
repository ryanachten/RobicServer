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
        public IActionResult Get([FromQuery(Name = "definition")] string definitionId)
        {
            if (definitionId != null)
            {
                return this.GetExercisesByDefintion(definitionId);
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var exercises = _exerciseRepository.GetUserExercises(userId);
            return Ok(exercises);
        }

        private IActionResult GetExercisesByDefintion(string definitionId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definition = _exerciseDefinitionContext.FindById(definitionId);
            if (definition == null)
                return NotFound();

            if (definition.User != userId)
                return Unauthorized();

            // Filter exercises to only those  associated with the user's definitions
            var exercises = _exerciseContext.AsQueryable()
                .Where(exercise => exercise.Definition == definitionId).ToList();
            return Ok(exercises);
        }

        [HttpGet("{id:length(24)}", Name = "GetExercise")]
        public async Task<IActionResult> GetExerciseById(string id)
        {
            Exercise exercise = await _exerciseContext.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            string userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (await isUserDefinition(exercise) == false)
                return Unauthorized();

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definition = await _exerciseDefinitionContext.FindByIdAsync(exercise.Definition);

            if (definition == null || definition.User != userId)
                return Unauthorized();

            // Currently setting exercise timestamp to system time
            // - TODO: invesitigate locale-specific solution
            exercise.Date = DateTime.Now;

            await _exerciseContext.InsertOneAsync(exercise);

            Exercise latestExercise = await _exerciseContext.FindByIdAsync(definition.History.LastOrDefault());

            // Add exercise to definition history
            definition.History.Add(exercise.Id);

            // Update definition aggregate fields
            definition.LastSession = exercise;
            if (latestExercise != null)
                definition.LastImprovement = ExerciseUtilities.GetLatestExerciseImprovement(exercise, latestExercise);

            await _exerciseDefinitionContext.ReplaceOneAsync(definition);

            return CreatedAtRoute("GetExercise", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Exercise updatedExercise)
        {
            if (await isUserDefinition(updatedExercise) == false)
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

        private async Task<bool> isUserDefinition(Exercise exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definiton = await _exerciseDefinitionContext.FindByIdAsync(exercise.Definition);
            return definiton != null && definiton.User == userId;
        }
    }
}