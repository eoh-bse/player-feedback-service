using System.Threading.Tasks;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service
{
    public interface IFeedbackSender
    {
        Result Send(PlayerFeedbackDto feedback);
    }
}
