using System;
using System.Collections.Immutable;

namespace PlayerFeedbackService.MessageBroker
{
    public class TopicNameProvider : ITopicNameProvider
    {
        private readonly IImmutableDictionary<Type, string> _topicNames;

        public TopicNameProvider(IImmutableDictionary<Type, string> topicNames)
        {
            _topicNames = topicNames;
        }

        public string ProvideFor<TMessage>()
        {
            var messageType = typeof(TMessage);

            if (_topicNames.ContainsKey(messageType))
            {
                return _topicNames[messageType];
            }

            throw new ArgumentException($"Topic name does not exist for message type '{messageType.Name}'");
        }
    }
}
