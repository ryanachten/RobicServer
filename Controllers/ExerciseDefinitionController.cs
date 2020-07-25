using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using System.Threading.Tasks;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseDefinitionController : ControllerBase
    {
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefintionRepo;

        public ExerciseDefinitionController(IMongoRepository<ExerciseDefiniton> exerciseDefintionRepo)
        {
            _exerciseDefintionRepo = exerciseDefintionRepo;
        }

        [HttpGet]
        public List<ExerciseDefiniton> Get() => _exerciseDefintionRepo.AsQueryable().ToList();

        [HttpGet("{id:length(24)}", Name = "GetExerciseDefinition")]
        public async Task<IActionResult> Get(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseDefiniton exercise)
        {
            await _exerciseDefintionRepo.InsertOneAsync(exercise);
            return CreatedAtRoute("GetExerciseDefinition", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ExerciseDefiniton updatedExercise)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await _exerciseDefintionRepo.ReplaceOneAsync(updatedExercise);
            return Ok(updatedExercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await _exerciseDefintionRepo.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}