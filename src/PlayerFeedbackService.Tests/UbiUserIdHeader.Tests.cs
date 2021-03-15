using Microsoft.AspNetCore.Http;
using Xunit;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Tests
{
    public class UbiUserIdHeaderTests
    {
        public class ExtractUbiUserIdShould
        {
            [Fact]
            public void ReturnOkWithUbiUserId_WhenHeaderIsPresent()
            {
                var ubiUserId = "player-id-1";
                var httpContext = new DefaultHttpContext();

                UbiUserIdHeaderHelper.AddUbiUserIdsToRequestHeader(httpContext.Request, ubiUserId);

                var result = UbiUserIdHeader.ExtractUbiUserId(httpContext.Request);

                Assert.True(result.IsOk);
                Assert.Equal(ubiUserId, result.Value);
            }

            [Fact]
            public void ReturnOkWithFirstUbiUserId_WhenHeaderIsPresentAndThereAreMultipleIds()
            {
                var ubiUserId1 = "player-id-1";
                var ubiUserId2 = "player-id-2";
                var httpContext = new DefaultHttpContext();

                UbiUserIdHeaderHelper.AddUbiUserIdsToRequestHeader(httpContext.Request, ubiUserId1, ubiUserId2);

                var result = UbiUserIdHeader.ExtractUbiUserId(httpContext.Request);

                Assert.True(result.IsOk);
                Assert.Equal(ubiUserId1, result.Value);
            }

            [Fact]
            public void ReturnError_WhenHeaderIsNotPresent()
            {
                var httpContext = new DefaultHttpContext();

                var result = UbiUserIdHeader.ExtractUbiUserId(httpContext.Request);

                var expectedError = new Error
                {
                    Type = UbiUserIdHeader.UbiUserIdMissingError,
                    Message = UbiUserIdHeader.UbiUserIdMissingErrorMessage
                };

                Assert.True(result.IsError);
                Assert.Equal(expectedError, result.Error);
            }

            [Fact]
            public void ReturnError_WhenHeaderIsPresentButEmpty()
            {
                var httpContext = new DefaultHttpContext();

                UbiUserIdHeaderHelper.AddUbiUserIdsToRequestHeader(httpContext.Request);

                var result = UbiUserIdHeader.ExtractUbiUserId(httpContext.Request);

                var expectedError = new Error
                {
                    Type = UbiUserIdHeader.UbiUserIdMissingError,
                    Message = UbiUserIdHeader.UbiUserIdMissingErrorMessage
                };

                Assert.True(result.IsError);
                Assert.Equal(expectedError, result.Error);
            }
        }
    }
}
