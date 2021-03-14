using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.Abstractions;

namespace PlayerFeedbackService.Controllers
{
    [ApiController]
    [Route("api/player-feedback")]
    public class PlayerFeedbackController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IFeedbackSender _feedbackSender;
        private readonly IClock _clock;
        private readonly ILogger<PlayerFeedbackController> _logger;

        public PlayerFeedbackController(
            IQueryHandler queryHandler,
            IFeedbackSender feedbackSender,
            IClock clock,
            ILogger<PlayerFeedbackController> logger
        )
        {
            _queryHandler = queryHandler;
            _feedbackSender = feedbackSender;
            _clock = clock;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerFeedbackResponse>>> Get([FromQuery] RawFilter filter = null)
        {
            var queryResult = await _queryHandler.GetLatestFeedBack(filter);

            if (queryResult.IsOk)
            {
                var response = queryResult.Value.Select(PlayerFeedbackResponse.CreateFromDto);

                return Ok(response);
            }

            return BadRequest(queryResult.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PlayerFeedBackRequest feedback)
        {
            var utcTimeNow = _clock.GetTimeNow();

            var feedbackSendingResult = await _feedbackSender.Send(feedback.ToDto(utcTimeNow));

            if (feedbackSendingResult.IsOk)
            {
                return Ok();
            }

            return BadRequest(feedbackSendingResult.Error);
        }
    }
}
