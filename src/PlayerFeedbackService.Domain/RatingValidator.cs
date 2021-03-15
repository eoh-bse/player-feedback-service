namespace PlayerFeedbackService.Domain
{
    public static class RatingValidator
    {
        public static bool IsOutOfRange(int? rating)
        {
            if (rating.HasValue)
            {
                return rating.Value < RatingRange.MinLimit || rating.Value > RatingRange.MaxLimit;
            }

            return true;
        }
    }
}
