using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

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

        public PlayerFeedbackController(
            IQueryHandler queryHandler,
            IFeedbackSender feedbackSender,
            IClock clock
        )
        {
            _queryHandler = queryHandler;
            _feedbackSender = feedbackSender;
            _clock = clock;
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
            var ubiUserIdExtractionResult = UbiUserIdHeader.ExtractUbiUserId(Request);

            if (ubiUserIdExtractionResult.IsError)
            {
                return BadRequest(ubiUserIdExtractionResult.Error);
            }

            var utcTimeNow = _clock.GetTimeNow();

            var feedbackSendingResult =
                await _feedbackSender.Send(feedback.ToDto(utcTimeNow, ubiUserIdExtractionResult.Value));

            if (feedbackSendingResult.IsOk)
            {
                return Ok();
            }

            return BadRequest(feedbackSendingResult.Error);
        }
    }
}
