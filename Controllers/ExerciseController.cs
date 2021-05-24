using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IExerciseDefinitionRepository _exerciseDefinitionRepo;

        public ExerciseController(IUnitOfWork unitOfWork)
        {
            _exerciseRepository = unitOfWork.ExerciseRepo;
            _exerciseDefinitionRepo = unitOfWork.ExerciseDefinitionRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDefinitionExercises([FromQuery(Name = "definition")] string definitionId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefinition definition = await _exerciseDefinitionRepo.GetExerciseDefinition(definitionId);
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
            var isUserExercise = await _exerciseDefinitionRepo.IsUsersDefinition(userID, exercise.Definition);
            if (!isUserExercise)
                return Unauthorized();

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExercise(Exercise exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefinition definition = await _exerciseDefinitionRepo.GetExerciseDefinition(exercise.Definition);

            if (definition == null || definition.User != userId)
                return Unauthorized();

            var createdExercise = await _exerciseRepository.CreateExercise(exercise, definition);

            return CreatedAtRoute("GetExercise", new { id = createdExercise.Id }, createdExercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateExercise(string id, Exercise updatedExercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isUserExercise = await _exerciseDefinitionRepo.IsUsersDefinition(userId, updatedExercise.Definition);
            if (!isUserExercise)
                return Unauthorized();

            Exercise exercise = await _exerciseRepository.GetExerciseById(id);
            if (exercise == null)
                return NotFound();

            await _exerciseRepository.UpdateExercise(updatedExercise);
            return Ok(updatedExercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteExercise(string id)
        {
            Exercise exercise = await _exerciseRepository.GetExerciseById(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefinition definiton = await _exerciseDefinitionRepo.GetExerciseDefinition(exercise.Definition);

            if (definiton == null || definiton.User != userId)
                return Unauthorized();

            await _exerciseRepository.DeleteExercise(id, definiton);
            return NoContent();
        }
    }
}