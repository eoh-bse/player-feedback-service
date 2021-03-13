using Xunit;

namespace PlayerFeedbackService.Domain.Tests
{
    public class RatingValidatorTests
    {
        public class RatingValidatorIsOutOfRangeShould
        {
            [Fact]
            public void ReturnTrue_WhenRatingIsNull()
            {
                var result = RatingValidator.IsOutOfRange(null);

                Assert.True(result);
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(6)]
            [InlineData(-30)]
            [InlineData(100)]
            public void ReturnTrue_WhenRatingIsLessThan1AndGreaterThan5(int rating)
            {
                var result = RatingValidator.IsOutOfRange(rating);

                Assert.True(result);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            public void ReturnFalse_WhenRatingIsFrom1To5(int rating)
            {
                var result = RatingValidator.IsOutOfRange(rating);

                Assert.False(result);
            }
        }
    }
}
