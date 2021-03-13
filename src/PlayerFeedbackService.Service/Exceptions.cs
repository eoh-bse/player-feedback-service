using System;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service
{
    public class DuplicateFeedbackException : Exception
    {
        private DuplicateFeedbackException(string message) : base(message) {}

        public static DuplicateFeedbackException Create(Guid sessionId, Guid playerId, DateTime timestamp)
        {
            var message = ErrorMessage.Create(ErrorType.DuplicateFeedback, sessionId, playerId, timestamp);

            return new DuplicateFeedbackException(message);
        }
    }
}
