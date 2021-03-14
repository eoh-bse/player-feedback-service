using System;
using System.Runtime.CompilerServices;
using PlayerFeedbackService.Service;

namespace PlayerFeedbackService
{
    public record PlayerFeedBackRequest
    {
        public Guid SessionId { get; init; }
        public Guid PlayerId { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; }

        public PlayerFeedbackDto ToDto(DateTime timestamp)
        {
            return new PlayerFeedbackDto
            {
                SessionId = SessionId,
                PlayerId = PlayerId,
                Rating = Rating,
                Comment = Comment,
                Timestamp = timestamp
            };
        }
    }

    public record PlayerFeedbackResponse
    {
        public Guid SessionId { get; }
        public Guid PlayerId { get; }
        public int Rating { get; }
        public string Comment { get; }

        public PlayerFeedbackResponse(Guid sessionId, Guid playerId, int rating, string comment)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            Rating = rating;
            Comment = comment;
        }

        public static PlayerFeedbackResponse CreateFromDto(PlayerFeedbackDto dto)
        {
            return new(dto.SessionId, dto.PlayerId, dto.Rating, dto.Comment);
        }
    }
}
