using System.Threading.Tasks;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.Abstractions
{
    public interface IFeedbackSender
    {
        Task<Result> Send(PlayerFeedbackDto feedback);
    }
}
