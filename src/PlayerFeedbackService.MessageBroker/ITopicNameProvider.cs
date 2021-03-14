namespace PlayerFeedbackService.MessageBroker
{
    public interface ITopicNameProvider
    {
        string ProvideFor<TMessage>();
    }
}
