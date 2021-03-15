using System;
using PlayerFeedbackService.Service;
using Xunit;

namespace PlayerFeedbackService.DataAccess.Tests
{
    public class PlayerFeedbackDocumentTests
    {
        public class PlayerFeedbackDocumentCreateFromDtoShould
        {
            [Fact]
            public void CreateDocumentId_By_CombiningSessionIdAndPlayerId()
            {
                var playerFeedbackDto = new PlayerFeedbackDto
                {
                    SessionId = new Guid("1e4b4f69-472d-4190-bdb2-450e8ae6e36d"),
                    PlayerId = new Guid("5a2b36f3-284c-4433-afed-57e60a84e987"),
                    Rating = 5,
                    Comment = "",
                    Timestamp = new DateTime(2021, 3, 15)
                };

                var document = PlayerFeedbackDocument.CreateFromDto(playerFeedbackDto);

                var expectedDoc = new PlayerFeedbackDocument
                {
                    Id = $"{playerFeedbackDto.SessionId.ToString()}-{playerFeedbackDto.PlayerId.ToString()}",
                    SessionId = playerFeedbackDto.SessionId,
                    PlayerId = playerFeedbackDto.PlayerId,
                    Rating = playerFeedbackDto.Rating,
                    Comment = playerFeedbackDto.Comment,
                    Timestamp = playerFeedbackDto.Timestamp
                };

                Assert.Equal(expectedDoc, document);
            }
        }
    }
}
