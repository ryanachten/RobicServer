using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Models;
using RobicServer.Services;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseDefinitionController : ControllerBase
    {
        private readonly ExerciseDefinitionService _exerciseDefintionService;

        public ExerciseDefinitionController(ExerciseDefinitionService exerciseDefintionService)
        {
            _exerciseDefintionService = exerciseDefintionService;
        }

        [HttpGet]
        public async Task<List<ExerciseDefiniton>> Get() => await _exerciseDefintionService.Get();
    }
}