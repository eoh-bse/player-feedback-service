using System;
using System.Text.Json;
using Confluent.Kafka;

namespace PlayerFeedbackService.MessageBroker
{
    public class MessageDeserializer<TMessage> : IDeserializer<TMessage>
    {
        public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                throw new ArgumentException("Error while deserializing message: Message cannot be null");
            }

            return JsonSerializer.Deserialize<TMessage>(data);
        }
    }
}
