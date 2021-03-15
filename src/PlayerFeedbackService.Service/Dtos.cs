using System;

namespace PlayerFeedbackService.Service
{
    public record PlayerFeedbackDto
    {
        public Guid SessionId { get; init; }
        public string PlayerId { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
