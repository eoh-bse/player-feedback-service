using Xunit;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.Tests
{
    public class QueryFilterTests
    {
        private readonly QueryFilter _defaultFilter = new QueryFilter
        {
            RatingRange = new RatingRange
            {
                Min = 1,
                Max = 5
            }
        };

        public class QueryFilterDefault : QueryFilterTests
        {
            [Fact]
            public void ShouldSetMinAndMaxRating()
            {
                var filter = QueryFilter.Default();

                Assert.Equal(_defaultFilter, filter);
            }
        }

        public class QueryFilterBy : QueryFilterTests
        {
            [Fact]
            public void ShouldCreateDefaultFilter()
            {
                var filter = QueryFilter.By();

                Assert.Equal(_defaultFilter, filter);
            }
        }

        public class QueryFilterRating : QueryFilterTests
        {
            [Fact]
            public void ShouldSetRatingRange()
            {
                var ratingRange = new RatingRange
                {
                    Min = 2,
                    Max = 4
                };

                var filter = QueryFilter.By().Rating(ratingRange);
                var expectedFilter = new QueryFilter
                {
                    RatingRange = ratingRange
                };

                Assert.Equal(expectedFilter, filter);
            }
        }

        public class QueryFilterCreateFrom : QueryFilterTests
        {
            [Fact]
            public void ShouldReturnOkAndDefaultFilter_WhenRawFilterIsNull()
            {
                var filterCreationResult = QueryFilter.CreateFrom(null);

                Assert.True(filterCreationResult.IsOk);
                Assert.Equal(_defaultFilter, filterCreationResult.Value);
            }

            [Fact]
            public void ShouldReturnOkAndDefaultFilter_WhenRawFilterIsEmpty()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = null,
                    MaxRating = null
                };

                var filterCreationResult = QueryFilter.CreateFrom(rawFilter);

                Assert.True(filterCreationResult.IsOk);
                Assert.Equal(_defaultFilter, filterCreationResult.Value);
            }

            [Theory]
            [InlineData(1, 5)]
            [InlineData(2, 4)]
            [InlineData(5, 5)]
            public void ShouldReturnFilter_WhenValidRatingRangeIsGiven(int? min, int? max)
            {
                var rawFilter = new RawFilter
                {
                    MinRating = min,
                    MaxRating = max
                };

                var filterCreationResult = QueryFilter.CreateFrom(rawFilter);
                var expectedFilter = new QueryFilter
                {
                    RatingRange = new RatingRange
                    {
                        Min = min.Value,
                        Max = max.Value
                    }
                };

                Assert.True(filterCreationResult.IsOk);
                Assert.Equal(expectedFilter, filterCreationResult.Value);
            }

            [Theory]
            [InlineData(-1, 5)]
            [InlineData(2, 9)]
            [InlineData(null, 5)]
            [InlineData(2, null)]
            public void ShouldReturnError_WhenMinMaxRatingIsOutOfRange(int? min, int? max)
            {
                var rawFilter = new RawFilter
                {
                    MinRating = min,
                    MaxRating = max
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
