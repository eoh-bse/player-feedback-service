using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.MessageBroker.Messages;
using PlayerFeedbackService.MessageBroker.MessageHandlers;
using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.MessageBroker.Tests.MessageHandlers
{
    public class AddPlayerFeedbackMessageHandlersTests
    {
        public class StorePlayerFeedbackShould
        {
            private readonly Mock<IPlayerFeedbackRepository> _mockPlayerFeedbackRepository;
            private readonly Mock<ILogger<AddPlayerFeedbackMessageHandlers>> _mockLogger;
            private readonly AddPlayerFeedbackMessage _defaultMessage = new AddPlayerFeedbackMessage
            {
                SessionId = new Guid("247a1b38-cf3f-4205-b278-2f2dafe1e843"),
                PlayerId = "player-id-1",
                Rating = 5,
                Comment = "",
                Timestamp = new DateTime(2021, 3, 15)
            };

            public StorePlayerFeedbackShould()
            {
                _mockPlayerFeedbackRepository = new Mock<IPlayerFeedbackRepository>();
                _mockLogger = new Mock<ILogger<AddPlayerFeedbackMessageHandlers>>();
            }

            [Fact]
            public void StorePlayerFeedback()
            {
                var playerFeedbackDto = new PlayerFeedbackDto
                {
                    SessionId = _defaultMessage.SessionId,
                    PlayerId = _defaultMessage.PlayerId,
                    Rating = _defaultMessage.Rating,
                    Comment = _defaultMessage.Comment,
                    Timestamp = _defaultMessage.Timestamp
                };

                _mockPlayerFeedbackRepository
                    .Setup(repo => repo.Store(It.IsAny<PlayerFeedbackDto>()))
                    .Verifiable();

                var messageHandlers =
                    new AddPlayerFeedbackMessageHandlers(_mockPlayerFeedbackRepository.Object, _mockLogger.Object);

                messageHandlers.StorePlayerFeedback(_defaultMessage);

                _mockPlayerFeedbackRepository.Verify(repo => repo.Store(playerFeedbackDto), Times.Once);
            }

            [Fact]
            public void LogsWarning_WhenDuplicateFeedbackExceptionIsThrown()
            {
                var exception = DuplicateFeedbackException.Create(
                    _defaultMessage.SessionId,
                    _defaultMessage.PlayerId,
                    _defaultMessage.Timestamp
                );

                _mockPlayerFeedbackRepository
                    .Setup(repo => repo.Store(It.IsAny<PlayerFeedbackDto>()))
                    .ThrowsAsync(exception);

                var messageHandlers =
                    new AddPlayerFeedbackMessageHandlers(_mockPlayerFeedbackRepository.Object, _mockLogger.Object);

                messageHandlers.StorePlayerFeedback(_defaultMessage);

                _mockLogger.Verify(logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<DuplicateFeedbackException>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ));
            }
        }
    }
}
