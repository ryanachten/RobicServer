using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Models;
using RobicServer.Services;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly ExerciseService _exerciseService;

        public ExerciseController(ExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public ActionResult<List<Exercise>> Get() => _exerciseService.Get();
    }
}