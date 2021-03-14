using System.Threading.Tasks;
using System.Collections.Generic;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.Abstractions
{
    public interface IQueryHandler
    {
        Task<Result<IReadOnlyCollection<PlayerFeedbackDto>>> GetLatestFeedBack(RawFilter filter);
    }
}
