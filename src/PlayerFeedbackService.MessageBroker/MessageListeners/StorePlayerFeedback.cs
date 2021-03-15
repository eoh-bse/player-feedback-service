using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

using PlayerFeedbackService.Service.MessageBroker;
using PlayerFeedbackService.Service.MessageBroker.Messages;
using PlayerFeedbackService.MessageBroker.MessageHandlers.Abstractions;

namespace PlayerFeedbackService.MessageBroker.MessageListeners
{
    public class StorePlayerFeedback : BackgroundService
    {
        private readonly IAddPlayerFeedbackMessageHandlers _messageHandlers;
        private readonly IMessageBrokerConfigProvider _messageBrokerConfigProvider;
        private readonly IMessageSender _dlqMessageSender;
        private readonly IDeserializer<AddPlayerFeedbackMessage> _messageDeserializer;
        private readonly ILogger<StorePlayerFeedback> _logger;

        public StorePlayerFeedback(
            IAddPlayerFeedbackMessageHandlers messageHandlers,
            IMessageBrokerConfigProvider messageBrokerConfigProvider,
            IMessageSender dlqMessageHandler,
            ILogger<StorePlayerFeedback> logger
        )
        {
            _messageHandlers = messageHandlers;
            _messageBrokerConfigProvider = messageBrokerConfigProvider;
            _dlqMessageSender = dlqMessageHandler;
            _messageDeserializer = new MessageDeserializer<AddPlayerFeedbackMessage>();
            _logger = logger;
        }

        private async Task StartListening(CancellationToken stoppingToken)
        {
            var brokerConfig = _messageBrokerConfigProvider.ProvideConfig();
            var topic = _messageBrokerConfigProvider.ProvideTopicFor<AddPlayerFeedbackMessage>();
            var groupId = topic.GroupIds[nameof(StorePlayerFeedback)];

            var config = new ConsumerConfig
            {
                BootstrapServers = brokerConfig.Servers,
                GroupId = groupId
            };

            using var consumer =
                new ConsumerBuilder<Null, AddPlayerFeedbackMessage>(config)
                    .SetValueDeserializer(_messageDeserializer)
                    .Build();

            consumer.Subscribe(topic.Name);

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);

                var message = consumeResult.Message.Value;

                try
                {
                    await _messageHandlers.StorePlayerFeedback(message);
                }
                catch (Exception ex)
                {
                    var serializedMessage = JsonSerializer.Serialize(message);

                    await _dlqMessageSender.SendToDeadLetterQueue(message, ex);

                    _logger.LogError(
                        $"Unexpected error occured. Sent message: {serializedMessage} to dead letter queue."
                    );
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartListening(stoppingToken));
        }
    }
}
