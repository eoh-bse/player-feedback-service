using System.Collections.Generic;

namespace PlayerFeedbackService.MessageBroker
{
    public interface IMessageBrokerConfigProvider
    {
        MessageBrokerConfig ProvideConfig();
        Topic ProvideTopicFor<TMessage>();
        IReadOnlyCollection<Topic> GetAllTopics();
    }
}
