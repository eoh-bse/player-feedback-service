using System;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service
{
    public class DuplicateFeedbackException : Exception
    {
        private DuplicateFeedbackException(string message) : base(message) {}

        public static DuplicateFeedbackException Create(Guid sessionId, string playerId, DateTime timestamp)
        {
            var message = ErrorMessage.Create(ErrorType.DuplicateFeedback, sessionId, playerId, timestamp);

            return new DuplicateFeedbackException(message);
        }
    }

    public class UnexpectedFeedbackInsertionException : Exception
    {
        public UnexpectedFeedbackInsertionException(string message, Exception innerEx) : base(message, innerEx) {}
    }
}
