using System;
using Nest;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service;

namespace PlayerFeedbackService.DataAccess
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public record PlayerFeedbackDocument
    {
        public string Id { get; init; }
        public Guid SessionId { get; init; }
        public Guid PlayerId { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; }
        public DateTime Timestamp { get; init; }

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

        public static PlayerFeedbackDocument CreateFromDto(PlayerFeedbackDto feedback)
        {
            return new PlayerFeedbackDocument
            {
                Id = $"{feedback.SessionId.ToString()}-{feedback.PlayerId.ToString()}",
                SessionId = feedback.SessionId,
                PlayerId = feedback.PlayerId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                Timestamp = feedback.Timestamp
            };
        }
    }
}
