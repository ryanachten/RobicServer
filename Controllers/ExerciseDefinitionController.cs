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
using System;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseDefinitionController : ControllerBase
    {
        private readonly IMongoRepository<Exercise> _exerciseRepo;
        private readonly IMongoRepository<ExerciseDefiniton> _exerciseDefintionRepo;
        private readonly IMongoRepository<User> _userRepo;
        private readonly IMapper _mapper;

        public ExerciseDefinitionController(
            IMongoRepository<Exercise> exerciseRepo,
            IMongoRepository<ExerciseDefiniton> exerciseDefintionRepo,
            IMongoRepository<User> userRepo,
            IMapper mapper
        )
        {
            _exerciseRepo = exerciseRepo;
            _exerciseDefintionRepo = exerciseDefintionRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public List<ExerciseDefinitionForListDto> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var definitons = _exerciseDefintionRepo.AsQueryable().Where(defintion => defintion.User == userId).ToList();
            var definitionsForReturn = _mapper.Map<List<ExerciseDefinitionForListDto>>(definitons);
            var definitionIds = definitons.Select(d => d.Id);

            var exercises = _exerciseRepo.AsQueryable().Where(e => definitionIds.Contains(e.Definition)).OrderByDescending(d => d.Date);

            // We assign the date of the latest exercise as the timestamp for last active
            definitionsForReturn.ForEach((definition) =>
            {
                var definitionExercises = exercises.Where(e => e.Definition == definition.Id);
                // FIXME: the following aggregates still cause massive performance penalties
                // definition.LastSession = definitionExercises.FirstOrDefault();
                // definition.LastImprovement = _utils.GetLatestExerciseImprovement(definition.Id, definitionExercises);
            });
            return definitionsForReturn;
        }

        [HttpGet("{id:length(24)}", Name = "GetExerciseDefinition")]
        public async Task<IActionResult> Get(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var exercises = _exerciseRepo.AsQueryable().Where(e => e.Definition == id);
            var utils = new ExerciseUtilities(exercises);
            exercise.LastSession = exercises.FirstOrDefault();
            exercise.LastImprovement = utils.GetLatestExerciseImprovement(exercise.Id);
            exercise.PersonalBest = utils.GetPersonalBest(exercise.Id);

            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseDefiniton exercise)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefintionRepo.InsertOneAsync(exercise);

            // Update user's exercises with new exercise
            User user = await _userRepo.FindByIdAsync(userId);
            user.Exercises.Add(exercise.Id);
            await _userRepo.ReplaceOneAsync(user);

            return CreatedAtRoute("GetExerciseDefinition", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ExerciseDefiniton updatedExercise)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            exercise.Title = updatedExercise.Title;
            exercise.Unit = updatedExercise.Unit;
            exercise.PrimaryMuscleGroup = updatedExercise.PrimaryMuscleGroup;

            await _exerciseDefintionRepo.ReplaceOneAsync(exercise);
            return Ok(exercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefintionRepo.DeleteByIdAsync(id);

            // Remove definition from user exercises
            User user = await _userRepo.FindByIdAsync(userId);
            user.Exercises.Remove(exercise.Id);
            await _userRepo.ReplaceOneAsync(user);

            // Remove exercises associated with definition
            await _exerciseRepo.DeleteManyAsync(e => e.Definition == id);

            return NoContent();
        }
    }
}