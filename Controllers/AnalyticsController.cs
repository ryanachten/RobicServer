using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using RobicServer.Query;
using RobicServer.Services;

namespace RobicServer.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPredictionService _predictor;

        public AnalyticsController(
            IMediator mediator,
            IPredictionService predictor
        )
        {
            _mediator = mediator;
            _predictor = predictor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var analytics = await _mediator.Send(new GetAnalytics
            {
                UserId = userId
            });
            return Ok(analytics);
        }

        [HttpGet("predict/{id}")]
        public async Task<IActionResult> Predict(string id)
        {
            // TODO: prediction service should be called via MediatR
            var results = await _predictor.PredictNetValue(id);
            return Ok(results);
        }
    }
}