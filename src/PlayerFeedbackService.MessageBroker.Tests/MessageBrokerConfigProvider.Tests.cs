using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace PlayerFeedbackService.MessageBroker.Tests
{
    public class MessageBrokerConfigProviderTests
    {
        private readonly MessageBrokerConfig _defaultConfig = new MessageBrokerConfig
        {
            Servers = "messagebroker:9092"
        };

        public class ProvideConfigShould : MessageBrokerConfigProviderTests
        {
            private readonly ImmutableDictionary<string, Topic> _defaultTopics =
                new Dictionary<string, Topic>().ToImmutableDictionary();

            [Fact]
            public void ProvideConfigItWasGiven()
            {
                var configProvider = new MessageBrokerConfigProvider(_defaultConfig, _defaultTopics);
                var returnedConfig = configProvider.ProvideConfig();

                Assert.Equal(_defaultConfig, returnedConfig);
            }
        }

        public class ProvideTopicForShould : MessageBrokerConfigProviderTests
        {
            public record TestMessage
            {
                public string MessageName { get; init; }
            }

            private readonly Topic _defaultTopic = new Topic
            {
                Name = "Test",
                For = typeof(TestMessage).Name,
                DeadLetterQueueName = "DLQ_Test",
                GroupIds = new Dictionary<string, string>().ToImmutableDictionary()
            };

            [Fact]
            public void ReturnCorrectTopic_ForTheGivenMessageType()
            {
                var topics = new Dictionary<string, Topic>
                {
                    { typeof(TestMessage).Name, _defaultTopic }
                }.ToImmutableDictionary();

                var configProvider = new MessageBrokerConfigProvider(_defaultConfig, topics);
                var returnedTopic = configProvider.ProvideTopicFor<TestMessage>();

                Assert.Equal(_defaultTopic, returnedTopic);
            }

            [Fact]
            public void ThrowArgumentException_WhenGivenMessageTypeDoesNotHaveTopic()
            {
                var topics = new Dictionary<string, Topic>().ToImmutableDictionary();

                var configProvider = new MessageBrokerConfigProvider(_defaultConfig, topics);

                Assert.Throws<ArgumentException>(() => configProvider.ProvideTopicFor<TestMessage>());
            }
        }
    }
}
