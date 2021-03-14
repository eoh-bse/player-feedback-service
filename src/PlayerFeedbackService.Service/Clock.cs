using System;

using PlayerFeedbackService.Service.Abstractions;

namespace PlayerFeedbackService.Service
{
    public class Clock : IClock
    {
        public DateTime GetTimeNow()
        {
            return DateTime.UtcNow;
        }
    }
}
