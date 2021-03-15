using System;
using Xunit;

namespace PlayerFeedbackService.Domain.Tests
{
    public class PlayerFeedbackTests
    {
        public class PlayerFeedbackCreateShould
        {
            private readonly Guid _sessionId = new Guid("c8ec99fe-f069-497c-a2e5-2ee998ead47f");
            private readonly string _playerId = "player-id-1";
            private readonly string comment = "player comment";
            private readonly DateTime timestamp = new DateTime(2021, 3, 12);

            [Fact]
            public void ReturnOk_WhenValidPropertiesAreGiven()
            {
                var rating = 5;
                var feedbackCreationResult = PlayerFeedback.Create(_sessionId, _playerId, rating, comment, timestamp);

                var expectedFeedback = new PlayerFeedback
                {
                    SessionId = _sessionId,
                    PlayerId = _playerId,
                    Rating = rating,
                    Comment = comment,
                    Timestamp = timestamp
                };

                Assert.True(feedbackCreationResult.IsOk);
                Assert.Equal(expectedFeedback, feedbackCreationResult.Value);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(33)]
            public void ResultError_WhenRatingIsOutOfRange(int rating)
            {
                var feedbackCreationResult = PlayerFeedback.Create(_sessionId, _playerId, rating, comment, timestamp);

                var expectedError = new Error
                {
                    Type = ErrorType.RatingOutOfRange.ToString(),
                    Message = ErrorMessage.RatingOutOfRangeMessage
                };

                Assert.True(feedbackCreationResult.IsError);
                Assert.Equal(expectedError, feedbackCreationResult.Error);
            }
        }
    }
}
