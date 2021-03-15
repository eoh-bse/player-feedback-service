using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PlayerFeedbackService.MessageBroker
{
    public class MessageBrokerConfigProvider : IMessageBrokerConfigProvider
    {
        private readonly MessageBrokerConfig _config;
        private readonly IImmutableDictionary<string, Topic> _topics;

        public MessageBrokerConfigProvider(MessageBrokerConfig config, IImmutableDictionary<string, Topic> topics)
        {
            _config = config;
            _topics = topics;
        }

        public MessageBrokerConfig ProvideConfig()
        {
            return _config;
        }

        public Topic ProvideTopicFor<TMessage>()
        {
            var messageType = typeof(TMessage).Name;

            if (_topics.ContainsKey(messageType))
            {
                return _topics[messageType];
            }

            throw new ArgumentException($"Topic name does not exist for message type '{messageType}'");
        }
    }
}
