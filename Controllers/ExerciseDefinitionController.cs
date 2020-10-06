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
            var exercises = _exerciseDefintionRepo.AsQueryable().Where(defintion => defintion.User == userId).ToList();
            var exerciseForReturn = _mapper.Map<List<ExerciseDefinitionForListDto>>(exercises);

            // We assign the date of teh latest exercise as the timestamp for last modified
            exerciseForReturn.ForEach((definition) =>
            {
                var latestExercise = _exerciseRepo.AsQueryable().Where(exercise => exercise.Definition == definition.Id).OrderByDescending(d => d.Date).FirstOrDefault();
                if (latestExercise != null)
                {
                    definition.LastActive = latestExercise.Date;
                }
            });
            return exerciseForReturn;
        }

        [HttpGet("{id:length(24)}", Name = "GetExerciseDefinition")]
        public async Task<IActionResult> Get(string id)
        {
            ExerciseDefiniton exercise = await _exerciseDefintionRepo.FindByIdAsync(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

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

            await _exerciseDefintionRepo.ReplaceOneAsync(updatedExercise);
            return Ok(updatedExercise);
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