using System;
using System.Threading.Tasks;
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

        private async Task Send(PlayerFeedback feedback)
        {
            try
            {
                await _playerFeedbackRepository.Store(feedback);
            }
            catch (DuplicateFeedbackException ex)
            {
                _logger.LogWarning(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error", ex);
            }
        }

        public Result Send(PlayerFeedbackDto feedback)
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
                Send(feedbackValidation.Value); // fire and forget

                return Result.Ok();
            }

            return Result.Fail(feedbackValidation.Error);
        }
    }
}
