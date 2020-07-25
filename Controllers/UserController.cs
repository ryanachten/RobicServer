using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RobicServer.Services;
using RobicServer.Models;
using AutoMapper;
using RobicServer.Models.DTOs;

namespace RobicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMongoRepository<User> _userRepo;

        public UserController(IMongoRepository<User> userRepo, IMapper mapper)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public async Task<UserForDetailDto> Get(string id)
        {
            User user = await _userRepo.FindByIdAsync(id);
            var userForReturn = _mapper.Map<UserForDetailDto>(user);
            return userForReturn;
        }
    }
}