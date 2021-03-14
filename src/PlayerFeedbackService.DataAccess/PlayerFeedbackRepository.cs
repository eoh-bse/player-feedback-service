using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Nest;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.DataAccess
{
    public class PlayerFeedbackRepository : IPlayerFeedbackRepository
    {
        public PlayerFeedbackRepository()
        {

        }

        public Task<IEnumerable<PlayerFeedbackDto>> GetLatestBy(QueryFilter filter)
        {
            return Task.Run(() => Array.Empty<PlayerFeedbackDto>().AsEnumerable());
        }

        public Task Store(PlayerFeedback feedback)
        {
            return Task.Run(() => {});
        }
    }
}
