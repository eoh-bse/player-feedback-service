using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service.Abstractions;
using PlayerFeedbackService.Service.MessageBroker;
using PlayerFeedbackService.Service.MessageBroker.Messages;

namespace PlayerFeedbackService.Service
{
    public class FeedbackSender : IFeedbackSender
    {
        private readonly IMessageSender<Null, AddPlayerFeedbackMessage> _messageSender;

        public FeedbackSender(IMessageSender<Null, AddPlayerFeedbackMessage> messageSender)
        {
            _messageSender = messageSender;
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

            await _messageSender.Send(AddPlayerFeedbackMessage.CreateFromDomain(feedbackValidation.Value));

            return Result.Ok();
        }
    }
}
