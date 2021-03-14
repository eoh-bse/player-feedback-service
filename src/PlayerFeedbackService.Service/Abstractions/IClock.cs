using System;

namespace PlayerFeedbackService.Service.Abstractions
{
    public interface IClock
    {
        DateTime GetTimeNow();
    }
}
