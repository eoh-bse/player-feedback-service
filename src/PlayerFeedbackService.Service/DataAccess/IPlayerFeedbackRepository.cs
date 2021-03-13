using System.Threading.Tasks;
using System.Collections.Generic;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.DataAccess
{
    public interface IPlayerFeedbackRepository
    {
        Task<IEnumerable<PlayerFeedbackDto>> GetLatestBy(QueryFilter filter);
        Task Store(PlayerFeedback feedback);
    }
}
