using System;

namespace PlayerFeedbackService.Domain
{
    public record PlayerFeedback
    {
        public Guid SessionId { get; init; }
        public Guid PlayerId { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; }
        public DateTime Timestamp { get; init; }

        public PlayerFeedback() {}

        private PlayerFeedback(Guid sessionId, Guid playerId, int rating, string comment, DateTime timestamp)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            Rating = rating;
            Comment = comment;
            Timestamp = timestamp;
        }

        public static Result<PlayerFeedback> Create(
            Guid sessionId,
            Guid playerId,
            int rating,
            string comment,
            DateTime timestamp
        )
        {
            if (RatingValidator.IsOutOfRange(rating))
            {
                return Result.Fail<PlayerFeedback>(new Error(ErrorType.RatingOutOfRange));
            }

            return Result.Ok(new PlayerFeedback(sessionId, playerId, rating, comment, timestamp));
        }
    }
}
