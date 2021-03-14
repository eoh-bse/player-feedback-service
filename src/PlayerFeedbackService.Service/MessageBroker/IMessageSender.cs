using System.Threading.Tasks;

namespace PlayerFeedbackService.Service.MessageBroker
{
    public interface IMessageSender<TKey, in TMessage>
    {
        Task Send(TMessage message);
    }
}
