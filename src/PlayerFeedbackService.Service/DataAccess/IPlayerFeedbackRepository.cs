using System.Threading.Tasks;
using System.Collections.Generic;

using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service.DataAccess
{
    public interface IPlayerFeedbackRepository
    {
        Task<IReadOnlyCollection<PlayerFeedbackDto>> GetLatestBy(QueryFilter filter);
        Task Store(PlayerFeedback feedback);
    }
}
