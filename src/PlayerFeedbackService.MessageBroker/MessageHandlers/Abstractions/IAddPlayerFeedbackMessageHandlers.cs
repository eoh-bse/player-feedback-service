using System.Threading.Tasks;

using PlayerFeedbackService.Service.MessageBroker.Messages;

namespace PlayerFeedbackService.MessageBroker.MessageHandlers.Abstractions
{
    public interface IAddPlayerFeedbackMessageHandlers
    {
        public Task StorePlayerFeedback(AddPlayerFeedbackMessage message);
    }
}
