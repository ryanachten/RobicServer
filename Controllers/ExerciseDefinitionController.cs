using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;

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
    }
}