using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;

using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.Service.Tests
{
    public class QueryHandlerTests
    {
        private readonly Mock<IPlayerFeedbackRepository> _mockFeedbackRepository;

        public QueryHandlerTests()
        {
            _mockFeedbackRepository = new Mock<IPlayerFeedbackRepository>();
        }

        public class GetLatestFeedbackShould : QueryHandlerTests
        {
            private readonly PlayerFeedbackDto _playerFeedback;

            public GetLatestFeedbackShould()
            {
                _playerFeedback = new PlayerFeedbackDto
                {
                    SessionId = new Guid("1e4b4f69-472d-4190-bdb2-450e8ae6e36d"),
                    PlayerId = new Guid("5a2b36f3-284c-4433-afed-57e60a84e987"),
                    Rating = 5,
                    Comment = "",
                    Timestamp = new DateTime(2021, 3, 12)
                };
            }

            [Fact]
            public async Task ReturnPlayerFeedbacks_WhenValidFilterIsGiven()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = 3,
                    MaxRating = 3
                };

                _mockFeedbackRepository
                    .Setup(repo => repo.GetLatestBy(It.IsAny<QueryFilter>()))
                    .ReturnsAsync(new [] { _playerFeedback });

                var queryHandler = new QueryHandler(_mockFeedbackRepository.Object);

                var result = await queryHandler.GetLatestFeedBack(rawFilter);

                var expectedResult = new[] { _playerFeedback }.AsEnumerable();

                Assert.True(result.IsOk);
                Assert.Equal(expectedResult, result.Value);
            }

            [Fact]
            public async Task ReturnError_WhenInvalidFilterIsGiven()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = -1,
                    MaxRating = null
                };

                var queryHandler = new QueryHandler(_mockFeedbackRepository.Object);

                var result = await queryHandler.GetLatestFeedBack(rawFilter);

                Assert.True(result.IsError);
            }
        }
    }
}
