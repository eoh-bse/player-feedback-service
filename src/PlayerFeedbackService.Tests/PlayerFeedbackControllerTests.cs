using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.Abstractions;
using PlayerFeedbackService.Controllers;

namespace PlayerFeedbackService.Tests
{
    public class PlayerFeedbackControllerTests
    {
        private readonly Mock<IQueryHandler> _mockQueryHandler;
        private readonly Mock<IFeedbackSender> _mockFeedbackSender;
        private readonly Mock<IClock> _mockClock;

        public PlayerFeedbackControllerTests()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockFeedbackSender = new Mock<IFeedbackSender>();
            _mockClock = new Mock<IClock>();

            _mockClock.Setup(clock => clock.GetTimeNow()).Returns(new DateTime(2021, 3, 15));
        }

        public class GetShould : PlayerFeedbackControllerTests
        {
            private readonly PlayerFeedbackDto _defaultPlayerFeedbackDto = new PlayerFeedbackDto
            {
                SessionId = new Guid("906733a4-f2c9-44c1-9e03-b8a8c5393e18"),
                PlayerId = "player-id-1",
                Rating = 5,
                Comment = "",
                Timestamp = new DateTime(2021, 3, 15)
            };

            [Fact]
            public async Task ReturnOkWithFeedbacks_WhenValidFilterIsProvided()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = 2,
                    MaxRating = 3
                };

                var feedbacks = Array.AsReadOnly(new[] { _defaultPlayerFeedbackDto });

                _mockQueryHandler
                    .Setup(handler => handler.GetLatestFeedBack(It.IsAny<RawFilter>()))
                    .ReturnsAsync(Result.Ok<IReadOnlyCollection<PlayerFeedbackDto>>(new[] { _defaultPlayerFeedbackDto }));

                var controller =
                    new PlayerFeedbackController(_mockQueryHandler.Object, _mockFeedbackSender.Object, _mockClock.Object);

                var responseResult = await controller.Get(rawFilter);
                var expectedPlayerFeedback = new PlayerFeedbackResponse
                {
                    SessionId = _defaultPlayerFeedbackDto.SessionId,
                    PlayerId = _defaultPlayerFeedbackDto.PlayerId,
                    Rating = _defaultPlayerFeedbackDto.Rating,
                    Comment = _defaultPlayerFeedbackDto.Comment,
                    Timestamp = _defaultPlayerFeedbackDto.Timestamp
                };

                var response = Assert.IsType<OkObjectResult>(responseResult.Result);
                var receivedFeedbacks = Assert.IsAssignableFrom<IEnumerable<PlayerFeedbackResponse>>(response.Value);

                Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
                Assert.Equal(expectedPlayerFeedback, receivedFeedbacks.First());
            }

            [Fact]
            public async Task ReturnBadRequest_WhenInvalidFilterIsProvided()
            {
                var rawFilter = new RawFilter
                {
                    MinRating = 2,
                    MaxRating = 1
                };

                var expectedError = new Error
                {
                    Type = ErrorType.MinRatingGreaterThanMax.ToString(),
                    Message = ErrorMessage.Create(ErrorType.MinRatingGreaterThanMax)
                };

                _mockQueryHandler
                    .Setup(handler => handler.GetLatestFeedBack(It.IsAny<RawFilter>()))
                    .ReturnsAsync(Result.Fail<IReadOnlyCollection<PlayerFeedbackDto>>(expectedError));

                var controller =
                    new PlayerFeedbackController(_mockQueryHandler.Object, _mockFeedbackSender.Object, _mockClock.Object);

                var responseResult = await controller.Get(rawFilter);

                var response = Assert.IsType<BadRequestObjectResult>(responseResult.Result);
                var receivedError = Assert.IsAssignableFrom<Error>(response.Value);

                Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
                Assert.Equal(expectedError, receivedError);
            }
        }

        public class PostShould : PlayerFeedbackControllerTests
        {
            [Fact]
            public async Task ReturnOk_WhenValidFeedbackIsGivenWithUbiUserIdHeader_And_FeedbackIsSuccessfullySent()
            {
                var ubiUserId = "player-id-1";
                var requestBody = new PlayerFeedBackRequest
                {
                    SessionId = new Guid("906733a4-f2c9-44c1-9e03-b8a8c5393e18"),
                    Rating = 5,
                    Comment = ""
                };

                _mockFeedbackSender
                    .Setup(sender => sender.Send(It.IsAny<PlayerFeedbackDto>()))
                    .ReturnsAsync(Result.Ok());

                var controller =
                    new PlayerFeedbackController(_mockQueryHandler.Object, _mockFeedbackSender.Object, _mockClock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext();

                UbiUserIdHeaderHelper.AddUbiUserIdsToRequestHeader(
                    controller.ControllerContext.HttpContext.Request,
                    ubiUserId
                );

                var responseResult = await controller.Post(requestBody);
                var response = Assert.IsType<OkResult>(responseResult);

                Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            }

            [Fact]
            public async Task ReturnBadRequest_WhenUbiUserIdHeaderIsMissing()
            {
                var requestBody = new PlayerFeedBackRequest
                {
                    SessionId = new Guid("906733a4-f2c9-44c1-9e03-b8a8c5393e18"),
                    Rating = 5,
                    Comment = ""
                };

                var controller =
                    new PlayerFeedbackController(_mockQueryHandler.Object, _mockFeedbackSender.Object, _mockClock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext();

                var responseResult = await controller.Post(requestBody);
                var response = Assert.IsType<BadRequestObjectResult>(responseResult);
                var receivedError = Assert.IsAssignableFrom<Error>(response.Value);

                var expectedError = new Error
                {
                    Type = UbiUserIdHeader.UbiUserIdMissingError,
                    Message = UbiUserIdHeader.UbiUserIdMissingErrorMessage
                };

                Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
                Assert.Equal(expectedError, receivedError);
            }

            [Fact]
            public async Task ReturnBadRequest_WhenUbiUserIdIsGiven_But_InvalidFeedbackIsGiven()
            {
                var ubiUserId = "player-id-1";
                var requestBody = new PlayerFeedBackRequest
                {
                    SessionId = new Guid("906733a4-f2c9-44c1-9e03-b8a8c5393e18"),
                    Rating = -1,
                    Comment = ""
                };

                var expectedError = new Error
                {
                    Type = ErrorType.RatingOutOfRange.ToString(),
                    Message = ErrorMessage.RatingOutOfRangeMessage
                };

                _mockFeedbackSender
                    .Setup(sender => sender.Send(It.IsAny<PlayerFeedbackDto>()))
                    .ReturnsAsync(Result.Fail(expectedError));

                var controller =
                    new PlayerFeedbackController(_mockQueryHandler.Object, _mockFeedbackSender.Object, _mockClock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext();

                UbiUserIdHeaderHelper.AddUbiUserIdsToRequestHeader(
                    controller.ControllerContext.HttpContext.Request,
                    ubiUserId
                );

                var responseResult = await controller.Post(requestBody);
                var response = Assert.IsType<BadRequestObjectResult>(responseResult);
                var receivedError = Assert.IsAssignableFrom<Error>(response.Value);

                Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
                Assert.Equal(expectedError, receivedError);
            }
        }
    }
}
