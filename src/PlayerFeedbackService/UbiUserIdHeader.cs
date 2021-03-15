using System.Linq;
using Microsoft.AspNetCore.Http;
using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService
{
    public static class UbiUserIdHeader
    {
        public const string KeyName = "Ubi-UserId";

        public const string UbiUserIdMissingError = "Ubi-UserId Missing";
        public const string UbiUserIdMissingErrorMessage = "Ubi-UserId is not present in the request header";

        public static Result<string> ExtractUbiUserId(HttpRequest request)
        {
            if (request.Headers != null &&
                request.Headers.ContainsKey(UbiUserIdHeader.KeyName) &&
                request.Headers[KeyName].Count >= 1 &&
                !string.IsNullOrWhiteSpace(request.Headers[KeyName].First())
            )
            {
                var ubiUserId = request.Headers[KeyName].First();

                return Result.Ok(ubiUserId);
            }

            var error = new Error
            {
                Type = UbiUserIdMissingError,
                Message = UbiUserIdMissingErrorMessage
            };

            return Result.Fail<string>(error);
        }
    }
}
