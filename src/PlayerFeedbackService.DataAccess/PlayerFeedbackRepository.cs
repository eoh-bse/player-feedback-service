using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using Elasticsearch.Net;
using Nest;

using PlayerFeedbackService.Domain;
using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.DataAccess;

namespace PlayerFeedbackService.DataAccess
{
    public class PlayerFeedbackRepository : IPlayerFeedbackRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<PlayerFeedbackRepository> _logger;

        public PlayerFeedbackRepository(IElasticClient elasticClient, ILogger<PlayerFeedbackRepository> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<PlayerFeedbackDto>> GetLatestBy(QueryFilter filter)
        {
            var playerFeedbacks = await _elasticClient.SearchAsync<PlayerFeedbackDocument>(search =>
                search
                    .RequestConfiguration(request => request.DisableDirectStreaming())
                    .Query(query => query.Range(range =>
                        range
                            .Field(field => field.Rating)
                            .GreaterThanOrEquals(filter.RatingRange.Min)
                            .LessThanOrEquals(filter.RatingRange.Max)
                    ))
                    .Size(15)
                    .Sort(sorting =>
                        sorting.Descending(doc => doc.Timestamp)
                    )
            );

            return playerFeedbacks.Documents.Select(doc => doc.ToDto()).ToImmutableArray();
        }

        public async Task Store(PlayerFeedbackDto feedback)
        {
            var feedbackDocument = PlayerFeedbackDocument.CreateFromDto(feedback);
            var creationResult = await _elasticClient.CreateDocumentAsync(feedbackDocument);

            if (creationResult.IsValid) return;

            if (creationResult.ServerError.Error.Type == ElasticsearchError.DocumentVersionConflictError)
            {
                throw DuplicateFeedbackException.Create(feedback.SessionId, feedback.PlayerId, feedback.Timestamp);
            }

            var unexpectedError = ElasticsearchError.GenerateMessageForPlayerFeedbackInsertionError(feedback);

            throw new UnexpectedFeedbackInsertionException(unexpectedError, creationResult.OriginalException);
        }
    }
}
