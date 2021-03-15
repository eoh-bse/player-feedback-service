using System;
using System.Threading.Tasks;

namespace PlayerFeedbackService.Service.MessageBroker
{
    public interface IMessageSender
    {
        Task Send<TMessage>(TMessage message);
        Task SendToDeadLetterQueue<TMessage>(TMessage deadLetterMessage, Exception error);
    }
}
