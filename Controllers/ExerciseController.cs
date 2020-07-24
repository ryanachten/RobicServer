using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
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
        public List<Exercise> Get()
        {
            IQueryable<Exercise> query = _exerciseExpo.AsQueryable();
            return query.ToList();
        }

        [HttpGet("{id}")]
        public async Task<Exercise> Get(string id) => await _exerciseExpo.FindByIdAsync(id);
    }
}