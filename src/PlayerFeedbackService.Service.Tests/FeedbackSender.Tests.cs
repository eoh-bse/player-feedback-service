using System;
using System.Threading.Tasks;
using Xunit;
using Moq;

using PlayerFeedbackService.Service.MessageBroker;
using PlayerFeedbackService.Service.MessageBroker.Messages;

namespace PlayerFeedbackService.Service.Tests
{
    public class FeedbackSenderTests
    {
        private readonly Mock<IMessageSender> _mockMessageSender;

        public FeedbackSenderTests()
        {
            _mockMessageSender = new Mock<IMessageSender>();
        }

        public class SendShould : FeedbackSenderTests
        {
            private readonly PlayerFeedbackDto _playerFeedback;

            public SendShould()
            {
                _playerFeedback = new PlayerFeedbackDto
                {
                    SessionId = new Guid("1e4b4f69-472d-4190-bdb2-450e8ae6e36d"),
                    PlayerId = "player-id-1",
                    Rating = 5,
                    Comment = "",
                    Timestamp = new DateTime(2021, 3, 12)
                };
            }

            [Fact]
            public async Task ReturnOk_WhenValidPlayerFeedbackIsGiven()
            {
                var addPlayerFeedbackMessage = new AddPlayerFeedbackMessage
                {
                    SessionId = _playerFeedback.SessionId,
                    PlayerId = _playerFeedback.PlayerId,
                    Rating = _playerFeedback.Rating,
                    Comment = _playerFeedback.Comment,
                    Timestamp = _playerFeedback.Timestamp
                };

                _mockMessageSender
                    .Setup(sender => sender.Send(It.IsAny<AddPlayerFeedbackMessage>()))
                    .Verifiable();

                var feedbackSender = new FeedbackSender(_mockMessageSender.Object);

                var result = await feedbackSender.Send(_playerFeedback);

                _mockMessageSender.Verify(sender => sender.Send(addPlayerFeedbackMessage));

                Assert.True(result.IsOk);
            }

            [Fact]
            public async Task ReturnError_WhenInvalidPlayerFeedbackIsGiven()
            {
                var invalidFeedback = _playerFeedback with { Rating = 7 };

                var feedbackSender = new FeedbackSender(_mockMessageSender.Object);

                var result = await feedbackSender.Send(invalidFeedback);

                Assert.True(result.IsError);
            }
        }
    }
}
