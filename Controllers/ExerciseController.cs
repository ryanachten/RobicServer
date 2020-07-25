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

        [HttpGet("{id}", Name = "GetExercise")]
        public async Task<Exercise> Get(string id) => await _exerciseExpo.FindByIdAsync(id);

        [HttpPost]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            await _exerciseExpo.InsertOneAsync(exercise);
            return CreatedAtRoute("GetExercise", new { id = exercise.Id }, exercise);
        }
    }
}