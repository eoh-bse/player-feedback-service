using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.Service.Tests
{
    public class FeedbackSenderTests
    {
        private readonly Mock<IPlayerFeedbackRepository> _mockFeedbackRepository;
        private readonly Mock<ILogger<FeedbackSender>> _mockLogger;

        public FeedbackSenderTests()
        {
            _mockFeedbackRepository = new Mock<IPlayerFeedbackRepository>();
            _mockLogger = new Mock<ILogger<FeedbackSender>>();
        }

        public class SendShould : FeedbackSenderTests
        {
            private readonly PlayerFeedbackDto _playerFeedback;

            public SendShould()
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
            public async Task ReturnOk_WhenValidPlayerFeedbackIsGiven()
            {
                var playerFeedback = new PlayerFeedback
                {
                    SessionId = _playerFeedback.SessionId,
                    PlayerId = _playerFeedback.PlayerId,
                    Rating = _playerFeedback.Rating,
                    Comment = _playerFeedback.Comment,
                    Timestamp = _playerFeedback.Timestamp
                };

                var feedbackSender = new FeedbackSender(_mockFeedbackRepository.Object, _mockLogger.Object);

                var result = await feedbackSender.Send(_playerFeedback);

                _mockFeedbackRepository.Verify(repo => repo.Store(playerFeedback));

                Assert.True(result.IsOk);
            }

            [Fact]
            public async Task ReturnError_WhenInvalidPlayerFeedbackIsGiven()
            {
                var invalidFeedback = _playerFeedback with { Rating = 7 };

                var feedbackSender = new FeedbackSender(_mockFeedbackRepository.Object, _mockLogger.Object);

                var result = await feedbackSender.Send(invalidFeedback);

                Assert.True(result.IsError);
            }
        }
    }
}
