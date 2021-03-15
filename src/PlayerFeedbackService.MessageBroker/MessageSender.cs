using System;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

using PlayerFeedbackService.Service.MessageBroker;

namespace PlayerFeedbackService.MessageBroker
{
    public class MessageSender : IMessageSender
    {
        private readonly IMessageBrokerConfigProvider _messageBrokerConfigProvider;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<MessageSender> _logger;

        public MessageSender(
            IMessageBrokerConfigProvider messageBrokerConfigProvider,
            IProducer<Null, string> producer,
            ILogger<MessageSender> logger
        )
        {
            _messageBrokerConfigProvider = messageBrokerConfigProvider;
            _producer = producer;
            _logger = logger;
        }

        public async Task Send<TMessage>(TMessage message)
        {
            var topic = _messageBrokerConfigProvider.ProvideTopicFor<TMessage>();
            var serializedMessage = JsonSerializer.Serialize(message);

            try
            {
                await _producer.ProduceAsync(topic.Name, new Message<Null, string>
                {
                    Value = serializedMessage
                });

                _logger.LogInformation($"Successfully published message: {serializedMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not publish message: {serializedMessage}", ex);
            }
        }

        public async Task SendToDeadLetterQueue<TMessage>(TMessage deadLetterMessage, Exception error)
        {
            var topic = _messageBrokerConfigProvider.ProvideTopicFor<TMessage>();
            var serializedMessage = JsonSerializer.Serialize(deadLetterMessage);
            var serializedError = error.ToString();

            try
            {
                var headers = new Headers();
                var errorHeader = new Header("Error", Encoding.UTF8.GetBytes(serializedError));
                headers.Add(errorHeader);

                await _producer.ProduceAsync(topic.DeadLetterQueueName, new Message<Null, string>
                {
                    Value = serializedMessage,
                    Headers = headers
                });

                _logger.LogInformation($"Successfully published message: {serializedMessage} to dead letter queue");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not publish message: {serializedMessage} to dead letter queue", ex);
            }
        }
    }
}
