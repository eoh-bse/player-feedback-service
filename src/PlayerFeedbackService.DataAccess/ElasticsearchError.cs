using System.Text.Json;

using PlayerFeedbackService.Service;

namespace PlayerFeedbackService.DataAccess
{
    public static class ElasticsearchError
    {
        public const string DocumentVersionConflictError = "version_conflict_engine_exception";

        public static string GenerateMessageForPlayerFeedbackInsertionError(PlayerFeedbackDto feedback)
        {
            var serializedFeedback = JsonSerializer.Serialize(feedback);

            return $"Failed to insert PlayerFeedback: {serializedFeedback}";
        }
    }
}
