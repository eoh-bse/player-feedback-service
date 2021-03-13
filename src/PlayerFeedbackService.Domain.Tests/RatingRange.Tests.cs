using Xunit;

namespace PlayerFeedbackService.Domain.Tests
{
    public class RatingRangeTests
    {
        public class RatingRangeCreateShould
        {
            [Fact]
            public void ReturnOk_WhenValidMinMaxRatingsAreGiven()
            {
                var minRating = new int?(1);
                var maxRating = new int?(5);

                var ratingRangeCreationResult = RatingRange.Create(minRating, maxRating);

                var expectedRatingRange = new RatingRange
                {
                    Min = minRating.Value,
                    Max = maxRating.Value
                };

                Assert.True(ratingRangeCreationResult.IsOk);
                Assert.Equal(expectedRatingRange, ratingRangeCreationResult.Value);
            }

            [Theory]
            [InlineData(null, 3)]
            [InlineData(1, null)]
            [InlineData(null, null)]
            [InlineData(1, 7)]
            [InlineData(-5, 3)]
            [InlineData(-2, null)]
            [InlineData(null, 100)]
            public void ReturnError_WhenMinMaxRatingsAreNullOrOutOfRange(int? minRating, int? maxRating)
            {
                var ratingRangeCreationResult = RatingRange.Create(minRating, maxRating);

                var expectedError = new Error
                {
                    Type = ErrorType.RatingOutOfRange.ToString(),
                    Message = ErrorMessage.RatingOutOfRangeMessage
                };

                Assert.True(ratingRangeCreationResult.IsError);
                Assert.Equal(expectedError, ratingRangeCreationResult.Error);
            }

            [Theory]
            [InlineData(5, 3)]
            [InlineData(4, 1)]
            public void ReturnError_WhenRatingsAreInRangeButMinRatingIsGreaterThanMaxRating(int? minRating, int? maxRating)
            {
                var ratingRangeCreationResult = RatingRange.Create(minRating, maxRating);

                var expectedError = new Error
                {
                    Type = ErrorType.MinRatingGreaterThanMax.ToString(),
                    Message = ErrorMessage.MinRatingGreaterThanMax
                };

                Assert.True(ratingRangeCreationResult.IsError);
                Assert.Equal(expectedError, ratingRangeCreationResult.Error);
            }
        }
    }
}
