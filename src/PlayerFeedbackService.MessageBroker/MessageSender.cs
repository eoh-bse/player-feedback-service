using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

using PlayerFeedbackService.Service.MessageBroker;

namespace PlayerFeedbackService.MessageBroker
{
    public class MessageSender<TKey, TMessage> : IMessageSender<TKey, TMessage>
    {
        private readonly ITopicNameProvider _topicNameProvider;
        private readonly IProducer<TKey, TMessage> _producer;
        private readonly ILogger<MessageSender<TKey, TMessage>> _logger;

        public MessageSender(
            ITopicNameProvider topicNameProvider,
            IProducer<TKey, TMessage> producer,
            ILogger<MessageSender<TKey, TMessage>> logger
        )
        {
            _topicNameProvider = topicNameProvider;
            _producer = producer;
            _logger = logger;
        }

        public async Task Send(TMessage message)
        {
            var topicName = _topicNameProvider.ProvideFor<TMessage>();
            var serializedMessage = JsonSerializer.Serialize(message);

            try
            {
                await _producer.ProduceAsync(topicName, new Message<TKey, TMessage>
                {
                    Value = message
                });

                _logger.LogInformation($"Successfully published message: {serializedMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not publish message: {serializedMessage}", ex);
            }
        }
    }
}
