using Xunit;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.Tests
{
    public class QueryFilterTests
    {
        public class QueryFilterEmpty
        {
            [Fact]
            public void ShouldSetIsEmptyTrue()
            {
                var filter = QueryFilter.Empty();

                Assert.True(filter.IsEmpty);
            }
        }

        public class QueryFilterBy
        {
            [Fact]
            public void ShouldSetIsEmptyTrue()
            {
                var filter = QueryFilter.Empty();

                Assert.True(filter.IsEmpty);
            }
        }

        public class QueryFilterRating
        {
            [Fact]
            public void ShouldSet_IsEmptyFalse_FilterByRatingTrue_And_RatingRange()
            {
                var ratingRange = new RatingRange
                {
                    Min = 1,
                    Max = 5
                };

                var filter = QueryFilter.By().Rating(ratingRange);

                Assert.False(filter.IsEmpty);
                Assert.True(filter.FilterByRating);
                Assert.Equal(ratingRange, filter.RatingRange);
            }
        }

        public class QueryFilterCreateFrom
        {
            [Fact]
            public void ShouldReturnEmptyFilter_WhenRawFilterIsEmpty()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = null,
                    MaxRating = null
                };

                var filterCreationResult = QueryFilter.CreateFrom(rawFilter);

                Assert.True(filterCreationResult.IsOk);
                Assert.True(filterCreationResult.Value.IsEmpty);
            }

            [Fact]
            public void ShouldReturnFilter_WhenValidRatingRangeIsGiven()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = 1,
                    MaxRating = 5
                };

                var filterCreationResult = QueryFilter.CreateFrom(rawFilter);

                var expectedRatingRange = new RatingRange
                {
                    Min = rawFilter.MinRating.Value,
                    Max = rawFilter.MaxRating.Value
                };

                Assert.True(filterCreationResult.IsOk);
                Assert.False(filterCreationResult.Value.IsEmpty);
                Assert.True(filterCreationResult.Value.FilterByRating);
                Assert.Equal(expectedRatingRange, filterCreationResult.Value.RatingRange);
            }

            [Fact]
            public void ShouldReturnError_WhenMinMaxRatingIsOutOfRange()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = null,
                    MaxRating = 9
                };

                var filterCreationResult = QueryFilter.CreateFrom(rawFilter);

                var expectedError = new Error
                {
                    Type = ErrorType.RatingOutOfRange.ToString(),
                    Message = ErrorMessage.RatingOutOfRangeMessage
                };

                Assert.True(filterCreationResult.IsError);
                Assert.Equal(expectedError, filterCreationResult.Error);
            }

            [Fact]
            public void ShouldReturnError_WhenMinRatingIsGreaterThanMax()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = 4,
                    MaxRating = 1
                };

                var filterCreationResult = QueryFilter.CreateFrom(rawFilter);

                var expectedError = new Error
                {
                    Type = ErrorType.MinRatingGreaterThanMax.ToString(),
                    Message = ErrorMessage.MinRatingGreaterThanMax
                };

                Assert.True(filterCreationResult.IsError);
                Assert.Equal(expectedError, filterCreationResult.Error);
            }
        }
    }
}
