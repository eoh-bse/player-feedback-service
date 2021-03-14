using System.Threading.Tasks;
using System.Collections.Generic;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service.Abstractions;
using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.Service
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPlayerFeedbackRepository _playerFeedback;

        public QueryHandler(IPlayerFeedbackRepository playerFeedback)
        {
            _playerFeedback = playerFeedback;
        }

        public async Task<Result<IReadOnlyCollection<PlayerFeedbackDto>>> GetLatestFeedBack(RawFilter filter)
        {
            var filterValidation = QueryFilter.CreateFrom(filter);

            if (filterValidation.IsOk)
            {
                var feedbacks = await _playerFeedback.GetLatestBy(filterValidation.Value);

                return Result.Ok(feedbacks);
            }

            return Result.Fail<IReadOnlyCollection<PlayerFeedbackDto>>(filterValidation.Error);
        }
    }
}
