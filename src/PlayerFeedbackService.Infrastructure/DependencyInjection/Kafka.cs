using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using PlayerFeedbackService.MessageBroker;

namespace PlayerFeedbackService.Infrastructure.DependencyInjection
{
    public static class KafkaExtensions
    {
        public static void AddMessageBrokerConfigProvider(
            this IServiceCollection services,
            MessageBrokerConfig brokerConfig,
            ImmutableDictionary<string, Topic> topics
        )
        {
            var messageBrokerConfigProvider = new MessageBrokerConfigProvider(brokerConfig, topics);

            services.AddSingleton<IMessageBrokerConfigProvider>(messageBrokerConfigProvider);
        }

        public static void AddKafkaProducer(this IServiceCollection services, Func<MessageBrokerConfig> createMessageBrokerConfig)
        {
            var messageBrokerConfig = createMessageBrokerConfig();
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = messageBrokerConfig.Servers
            };

            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            services.AddSingleton<IProducer<Null, string>>(producer);
        }

        public static void CreateTopicsOnMessageBroker(
            this IServiceCollection services,
            MessageBrokerConfig brokerConfig,
            ImmutableDictionary<string, Topic> topics
        )
        {
            var adminClientConfig = new AdminClientConfig
            {
                BootstrapServers = brokerConfig.Servers,
            };

            using var adminClient = new AdminClientBuilder(adminClientConfig).Build();

            var topicsToCreate =
                topics.Values.SelectMany(topic =>
                {
                    return new[]
                    {
                        new TopicSpecification
                        {
                            Name = topic.Name
                        },
                        new TopicSpecification
                        {
                            Name = topic.DeadLetterQueueName
                        }
                    };
                });

            var existingTopics = adminClient.GetMetadata(TimeSpan.FromSeconds(30.0)).Topics;
            var finalTopicsToCreate =
                topicsToCreate.Where(topic =>
                    !existingTopics.Any(existingTopic => existingTopic.Topic.Contains(topic.Name))
                ).ToImmutableArray();

            if (finalTopicsToCreate.Length > 0)
            {
                adminClient.CreateTopicsAsync(finalTopicsToCreate).Wait();
            }
        }
    }
}
