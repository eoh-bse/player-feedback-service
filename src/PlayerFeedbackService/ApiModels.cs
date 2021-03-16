using System;
using System.ComponentModel.DataAnnotations;

using PlayerFeedbackService.Service;

namespace PlayerFeedbackService
{
    public record PlayerFeedBackRequest
    {
        [Required]
        public Guid? SessionId { get; init; }

        [Required]
        public int? Rating { get; init; }

        [Required(AllowEmptyStrings = true)]
        public string Comment { get; init; }

        public PlayerFeedbackDto ToDto(DateTime timestamp, string playerId)
        {
            return new PlayerFeedbackDto
            {
                SessionId = SessionId.Value, // should be validated by this time
                PlayerId = playerId,
                Rating = Rating.Value, // should be validated by this time
                Comment = Comment,
                Timestamp = timestamp
            };
        }
    }

    public record PlayerFeedbackResponse
    {
        public Guid SessionId { get; init; }
        public string PlayerId { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; }
        public DateTime Timestamp { get; init; }

        public PlayerFeedbackResponse() {}

        public PlayerFeedbackResponse(Guid sessionId, string playerId, int rating, string comment, DateTime timestamp)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            Rating = rating;
            Comment = comment;
            Timestamp = timestamp;
        }

        public static PlayerFeedbackResponse CreateFromDto(PlayerFeedbackDto dto)
        {
            return new(dto.SessionId, dto.PlayerId, dto.Rating, dto.Comment, dto.Timestamp);
        }
    }
}
