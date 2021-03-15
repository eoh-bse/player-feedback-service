namespace PlayerFeedbackService.MessageBroker
{
    public interface IMessageBrokerConfigProvider
    {
        MessageBrokerConfig ProvideConfig();
        Topic ProvideTopicFor<TMessage>();
    }
}
