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

        [HttpGet("predict")]
        public async Task<IActionResult> Predict()
        {
            string definitionId = "5d0dac4391c2894536e6480d";

            _predictor.PredictNetValue(definitionId);
            return Ok("predict!");
        }
    }
}