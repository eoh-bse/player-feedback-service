using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service.Abstractions;
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

        public async Task<Result> Send(PlayerFeedbackDto feedback)
        {
            var feedbackValidation =
                PlayerFeedback.Create(
                    feedback.SessionId,
                    feedback.PlayerId,
                    feedback.Rating,
                    feedback.Comment,
                    feedback.Timestamp
                );

            if (!feedbackValidation.IsOk)
            {
                return Result.Fail(feedbackValidation.Error);

            }

            try
            {
                await _playerFeedbackRepository.Store(feedbackValidation.Value);

                return Result.Ok();
            }
            catch (DuplicateFeedbackException ex)
            {
                _logger.LogWarning(ex.Message);
            }
            catch (UnexpectedFeedbackInsertionException ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Result.Ok();
        }
    }
}
