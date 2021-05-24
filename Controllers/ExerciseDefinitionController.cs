using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using RobicServer.Models.DTOs;
using MediatR;
using RobicServer.Query;
using System;
using RobicServer.Command;

namespace RobicServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseDefinitionController : ControllerBase
    {
        private readonly IExerciseDefinitionRepository _exerciseDefinitionRepo;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ExerciseDefinitionController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator
        )
        {
            _exerciseDefinitionRepo = unitOfWork.ExerciseDefinitionRepo;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDefinition()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var definitions = await _mediator.Send(new GetExerciseDefinitions
                {
                    UserId = userId
                });
                var defintionsToReturn = _mapper.Map<List<ExerciseDefinitionForListDto>>(definitions);
                return Ok(defintionsToReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetExerciseDefinition")]
        public async Task<IActionResult> GetExeciseDefinition(string id)
        {
            try
            {
                var definition = await _mediator.Send(new GetExerciseDefinitionById
                {
                    ExerciseId = id
                });
                if (definition == null)
                    return NotFound();

                if (definition.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    return Unauthorized();

                return Ok(definition);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefinition(ExerciseDefinition exerciseToCreate)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                if (exerciseToCreate.User != userId)
                    return Unauthorized();

                var definition = await _mediator.Send(new CreateExerciseDefinition
                {
                    UserId = userId,
                    Definition = exerciseToCreate
                });

                return CreatedAtRoute("GetExerciseDefinition", new { id = definition.Id }, definition);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ExerciseDefinition updatedExercise)
        {
            ExerciseDefinition exercise = await _exerciseDefinitionRepo.GetExerciseDefinition(id);
            if (exercise == null)
                return NotFound();

            if (exercise.User != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            await _exerciseDefinitionRepo.UpdateDefinition(exercise, updatedExercise);

            return Ok(exercise);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            ExerciseDefinition exercise = await _exerciseDefinitionRepo.GetExerciseDefinition(id);
            if (exercise == null)
                return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (exercise.User != userId)
                return Unauthorized();

            await _exerciseDefinitionRepo.DeleteDefinition(userId, id);

            return NoContent();
        }
    }
}