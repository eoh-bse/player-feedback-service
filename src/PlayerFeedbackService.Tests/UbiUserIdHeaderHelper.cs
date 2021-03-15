using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace PlayerFeedbackService.Tests
{
    public static class UbiUserIdHeaderHelper
    {
        public static void AddUbiUserIdsToRequestHeader(HttpRequest request, params string[] ubiUserIds)
        {
            var headerKey = UbiUserIdHeader.KeyName;
            var headerValue = new StringValues(ubiUserIds);
            var header = KeyValuePair.Create(headerKey, headerValue);

            request.Headers.Add(header);
        }
    }
}
