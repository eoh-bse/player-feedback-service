namespace PlayerFeedbackService.Domain
{
    public record RatingRange
    {
        public static readonly int MinLimit = 1;
        public static readonly int MaxLimit = 5;
        public int Min { get; init; }
        public int Max { get; init; }

        public RatingRange() {}

        private RatingRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public static Result<RatingRange> Create(int? min, int? max)
        {
            if (RatingValidator.IsOutOfRange(min) || RatingValidator.IsOutOfRange(max))
            {
                return Result.Fail<RatingRange>(new Error(ErrorType.RatingOutOfRange));
            }

            if (min > max)
            {
                return Result.Fail<RatingRange>(new Error(ErrorType.MinRatingGreaterThanMax));
            }

            return Result.Ok(new RatingRange(min.Value, max.Value));
        }
    }
}
