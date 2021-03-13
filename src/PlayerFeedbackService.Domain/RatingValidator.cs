namespace PlayerFeedbackService.Domain
{
    public static class RatingValidator
    {
        public static bool IsOutOfRange(int? rating)
        {
            if (rating.HasValue)
            {
                return rating.Value < 1 || rating.Value > 5;
            }

            return true;
        }
    }
}
