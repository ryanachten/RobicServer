using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using RobicServer.Query;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IExerciseDefinitionRepository _exerciseDefinitionRepo;
        private readonly IMediator _mediator;

        public ExerciseController(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _exerciseRepository = unitOfWork.ExerciseRepo;
            _exerciseDefinitionRepo = unitOfWork.ExerciseDefinitionRepo;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDefinitionExercises([FromQuery(Name = "definition")] string definitionId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var definition = await _mediator.Send(new GetExerciseDefinitionById
            {
                DefinitionId = definitionId
            });
            if (definition == null)
                return NotFound();

            if (definition.User != userId)
                return Unauthorized();

            var exercises = await _mediator.Send(new GetExercisesByDefinition
            {
                DefinitionId = definitionId
            });
            return Ok(exercises);
        }

        [HttpGet("{id:length(24)}", Name = "GetExercise")]
        public async Task<IActionResult> GetExerciseById(string id)
        {
            var exercise = await _mediator.Send(new GetExerciseById
            {
                ExerciseId = id
            });
            if (exercise == null)
                return NotFound();

            var isUserExercise = await IsUsersDefinition(exercise.Definition);
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
            var isUserExercise = await IsUsersDefinition(updatedExercise.Definition);
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

        private async Task<bool> IsUsersDefinition(string definitionId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var definition = await _mediator.Send(new GetExerciseDefinitionById
            {
                DefinitionId = definitionId
            });
            return definition != null && definition.User == userId;
        }
    }
}