using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IMongoRepository<Exercise> _exerciseExpo;

        public ExerciseController(IMongoRepository<Exercise> exerciseExpo)
        {
            _exerciseExpo = exerciseExpo;
        }

        [HttpGet]
        public List<Exercise> Get() => _exerciseExpo.AsQueryable().ToList();

        [HttpGet("{id:length(24)}", Name = "GetExercise")]
        public async Task<IActionResult> Get(string id)
        {
            Exercise exercise = await _exerciseExpo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            await _exerciseExpo.InsertOneAsync(exercise);
            return CreatedAtRoute("GetExercise", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Exercise updatedExercise)
        {
            Exercise exercise = await _exerciseExpo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await _exerciseExpo.ReplaceOneAsync(updatedExercise);
            return Ok(exercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            Exercise exercise = await _exerciseExpo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await _exerciseExpo.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}