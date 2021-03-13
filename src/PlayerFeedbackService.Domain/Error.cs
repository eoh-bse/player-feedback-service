using System;
using System.Globalization;

namespace PlayerFeedbackService.Domain
{
    public enum ErrorType
    {
        RatingOutOfRange,
        MinRatingGreaterThanMax,
        DuplicateFeedback
    }

    public static class ErrorMessage
    {
        public const string RatingOutOfRangeMessage = "Player feedback rating is out of range. It must be from 1 to 5";
        public const string MinRatingGreaterThanMax = "Specified MinRating is greater than MaxRating";
        public const string DuplicateFeedbackMessage = "";
        public const string UnknownErrorMessage = "Unknown Error";

        private static string BuildForDuplicateFeedbackMessage(Guid sessionId, Guid playerId, DateTime timestamp)
        {
            var formattedDate = timestamp.ToString("u", DateTimeFormatInfo.InvariantInfo);

            return $"Player {playerId} in session {sessionId} attempted to send more than 1 feedback at {formattedDate}";
        }

        public static string Create(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.RatingOutOfRange:
                    return RatingOutOfRangeMessage;

                case ErrorType.MinRatingGreaterThanMax:
                    return MinRatingGreaterThanMax;

                case ErrorType.DuplicateFeedback:
                    return DuplicateFeedbackMessage;

                default:
                    return UnknownErrorMessage;
            }
        }

        public static string Create(ErrorType type, Guid sessionId, Guid playerId, DateTime timestamp)
        {
            if (type == ErrorType.DuplicateFeedback)
            {
                return BuildForDuplicateFeedbackMessage(sessionId, playerId, timestamp);
            }

            return Create(type);
        }
    }

    public record Error
    {
        public string Type { get; init; }
        public string Message { get; init; }

        public Error() {}

        public Error(ErrorType type)
        {
            Type = type.ToString();
            Message = ErrorMessage.Create(type);
        }
    }
}
