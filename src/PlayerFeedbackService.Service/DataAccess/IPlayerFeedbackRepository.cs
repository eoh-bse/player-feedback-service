using System.Threading.Tasks;
using System.Collections.Generic;

namespace PlayerFeedbackService.Service.DataAccess
{
    public interface IPlayerFeedbackRepository
    {
        Task<IReadOnlyCollection<PlayerFeedbackDto>> GetLatestBy(QueryFilter filter);
        Task Store(PlayerFeedbackDto feedback);
    }
}
