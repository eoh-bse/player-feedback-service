using System;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.MessageBroker.Messages
{
    public record AddPlayerFeedbackMessage
    {
        public Guid SessionId { get; init; }
        public Guid PlayerId { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; }
        public DateTime Timestamp { get; init; }

        public static AddPlayerFeedbackMessage CreateFromDomain(PlayerFeedback feedback)
        {
            return new AddPlayerFeedbackMessage
            {
                SessionId = feedback.SessionId,
                PlayerId = feedback.PlayerId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                Timestamp = feedback.Timestamp
            };
        }

        public PlayerFeedbackDto ToDto()
        {
            return new PlayerFeedbackDto
            {
                SessionId = SessionId,
                PlayerId = PlayerId,
                Rating = Rating,
                Comment = Comment,
                Timestamp = Timestamp
            };
        }
    }
}
