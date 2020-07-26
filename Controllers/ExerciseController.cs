using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IMongoRepository<Exercise> _exerciseRepo;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefinitionRepo;

        public ExerciseController(IMongoRepository<Exercise> exerciseRepo, IMongoRepository<ExerciseDefiniton> exerciseDefinitionRepo)
        {
            _exerciseRepo = exerciseRepo;
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
        }

        [HttpGet]
        public List<Exercise> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // Filter exercises to only those  associated with the user's definitions
            var exerciseDefinitionIds = _exerciseDefinitionRepo.AsQueryable()
                .Where(exercise => exercise.User == userId)
                .Select(exerciseDefinitions => exerciseDefinitions.Id).ToArray();
            return _exerciseRepo.AsQueryable().Where(exercise => exerciseDefinitionIds.Contains(exercise.Definition)).ToList();
        }

        [HttpGet("{id:length(24)}", Name = "GetExercise")]
        public async Task<IActionResult> Get(string id)
        {
            Exercise exercise = await _exerciseRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (await isUserDefinition(exercise) == false)
                return Unauthorized();

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            if (await isUserDefinition(exercise) == false)
                return Unauthorized();

            await _exerciseRepo.InsertOneAsync(exercise);
            return CreatedAtRoute("GetExercise", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Exercise updatedExercise)
        {
            if (await isUserDefinition(updatedExercise) == false)
                return Unauthorized();

            Exercise exercise = await _exerciseRepo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await _exerciseRepo.ReplaceOneAsync(updatedExercise);
            return Ok(updatedExercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            Exercise exercise = await _exerciseRepo.FindByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            if (await isUserDefinition(exercise) == false)
                return Unauthorized();

            await _exerciseRepo.DeleteByIdAsync(id);
            return NoContent();
        }

        private async Task<bool> isUserDefinition(Exercise exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ExerciseDefiniton definiton = await _exerciseDefinitionRepo.FindByIdAsync(exercise.Definition);
            return definiton != null && definiton.User == userId;
        }
    }
}