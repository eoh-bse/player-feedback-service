using System.Collections.Immutable;

namespace PlayerFeedbackService.MessageBroker
{
    public record Topic
    {
        public string Name { get; init; }
        public string For { get; init; } // Message type topic is responsible for
        public string DeadLetterQueueName { get; init; }
        public ImmutableDictionary<string, string> GroupIds { get; init; }
    }
}
