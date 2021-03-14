using System;

namespace PlayerFeedbackService.Infrastructure
{
    public record ElasticsearchConfig
    {
        public string DefaultIndex { get; init; }
        public Uri Uri { get; init; }
    }
}
