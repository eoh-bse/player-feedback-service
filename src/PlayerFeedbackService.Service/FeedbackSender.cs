using Microsoft.Extensions.Logging;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.Service
{
    public class FeedbackSender : IFeedbackSender
    {
        private readonly IPlayerFeedbackRepository _playerFeedbackRepository;
        private readonly ILogger<FeedbackSender> _logger;

        public FeedbackSender(IPlayerFeedbackRepository playerFeedbackRepository, ILogger<FeedbackSender> logger)
        {
            _playerFeedbackRepository = playerFeedbackRepository;
            _logger = logger;
        }

        public Result Send(PlayerFeedbackDto feedback)
        {
            try
            {
                var feedbackValidation =
                    PlayerFeedback.Create(
                        feedback.SessionId,
                        feedback.PlayerId,
                        feedback.Rating,
                        feedback.Comment,
                        feedback.Timestamp
                    );

                if (feedbackValidation.IsOk)
                {
                    _playerFeedbackRepository.Store(feedbackValidation.Value);

                    return Result.Ok();
                }

                return Result.Fail(feedbackValidation.Error);
            }
            catch (DuplicateFeedbackException ex)
            {
                _logger.LogWarning(ex.Message);

                return Result.Ok();
            }
        }
    }
}
