using System.Threading.Tasks;
using System.Collections.Generic;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service
{
    public interface IQueryHandler
    {
        Task<Result<IEnumerable<PlayerFeedbackDto>>> GetLatestFeedBack(RawFilter filter);
    }
}
