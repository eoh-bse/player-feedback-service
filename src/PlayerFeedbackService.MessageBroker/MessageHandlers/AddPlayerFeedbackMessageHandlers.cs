using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.DataAccess;
using PlayerFeedbackService.Service.MessageBroker.Messages;
using PlayerFeedbackService.MessageBroker.MessageHandlers.Abstractions;

namespace PlayerFeedbackService.MessageBroker.MessageHandlers
{
    public class AddPlayerFeedbackMessageHandlers : IAddPlayerFeedbackMessageHandlers
    {
        private readonly IPlayerFeedbackRepository _playerFeedbackRepository;
        private readonly ILogger<AddPlayerFeedbackMessageHandlers> _logger;

        public AddPlayerFeedbackMessageHandlers(
            IPlayerFeedbackRepository playerFeedbackRepository,
            ILogger<AddPlayerFeedbackMessageHandlers> logger
        )
        {
            _playerFeedbackRepository = playerFeedbackRepository;
            _logger = logger;
        }

        public async Task StorePlayerFeedback(AddPlayerFeedbackMessage message)
        {
            try
            {
                var playerFeedbackDto = message.ToDto();

                await _playerFeedbackRepository.Store(playerFeedbackDto);
            }
            catch (DuplicateFeedbackException ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}
