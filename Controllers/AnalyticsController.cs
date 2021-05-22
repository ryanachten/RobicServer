using Microsoft.AspNetCore.Mvc;
using RobicServer.Data;
using RobicServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RobicServer.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository _analyticsRepo;

        public AnalyticsController(
            IUnitOfWork unitOfWork
        )
        {
            _analyticsRepo = unitOfWork.AnalyticsRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Analytics analytics = _analyticsRepo.GetUserAnalytics(userId);
            return Ok(analytics);
        }
    }
}