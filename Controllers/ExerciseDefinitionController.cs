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
using RobicServer.Helpers;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseDefinitionController : ControllerBase
    {
        private readonly IExerciseDefinitionRepository _exerciseDefinitionRepo;
        private readonly IMongoRepository<Exercise> _exerciseContext;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefintionContext;
        private readonly IMongoRepository<User> _userContext;
        private readonly IMapper _mapper;

        public ExerciseDefinitionController(
            IExerciseDefinitionRepository exerciseDefinitionRepo,
            IMongoRepository<Exercise> exerciseContext,
            IMongoRepository<ExerciseDefiniton> exerciseDefintionContext,
            IMongoRepository<User> userContext,
            IMapper mapper
        )
        {
            _exerciseDefinitionRepo = exerciseDefinitionRepo;
            _exerciseContext = exerciseContext;
            _exerciseDefintionContext = exerciseDefintionContext;
            _userContext = userContext;
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
        public async Task<IActionResult> Get(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionContext.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var util = new ExerciseUtilities(this._exerciseContext.FilterBy(e => e.Definition == id).AsQueryable());
            exercise.PersonalBest = util.GetPersonalBest(id);

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseDefiniton exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefintionContext.InsertOneAsync(exercise);

            // Update user's exercises with new exercise
            User user = await _userContext.FindByIdAsync(userId);
            user.Exercises.Add(exercise.Id);
            await _userContext.ReplaceOneAsync(user);

            return CreatedAtRoute("GetExerciseDefinition", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ExerciseDefiniton updatedExercise)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionContext.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            exercise.Title = updatedExercise.Title;
            exercise.Unit = updatedExercise.Unit;
            exercise.PrimaryMuscleGroup = updatedExercise.PrimaryMuscleGroup;

            await _exerciseDefintionContext.ReplaceOneAsync(exercise);
            return Ok(exercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionContext.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefintionContext.DeleteByIdAsync(id);

            // Remove definition from user exercises
            User user = await _userContext.FindByIdAsync(userId);
            user.Exercises.Remove(exercise.Id);
            await _userContext.ReplaceOneAsync(user);

            // Remove exercises associated with definition
            await _exerciseContext.DeleteManyAsync(e => e.Definition == id);

            return NoContent();
        }
    }
}