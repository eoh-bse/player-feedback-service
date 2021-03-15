using PlayerFeedbackService.Domain;

namespace PlayerFeedbackService.Service
{
    public record RawFilter
    {
        public int? MinRating { get; init; }
        public int? MaxRating { get; init; }

        public bool IsEmpty()
        {
            return !MinRating.HasValue && !MaxRating.HasValue;
        }
    }

    public record QueryFilter
    {
        public RatingRange RatingRange { get; init; }

        public static QueryFilter Default()
        {
            return new()
            {
                RatingRange = new RatingRange
                {
                    Min = RatingRange.MinLimit,
                    Max = RatingRange.MaxLimit
                }
            };
        }

        public static QueryFilter By()
        {
            return Default();
        }

        public QueryFilter Rating(RatingRange ratingRange)
        {
            return this with { RatingRange = ratingRange };
        }

        public static Result<QueryFilter> CreateFrom(RawFilter filter)
        {
            if (filter == null || filter.IsEmpty())
            {
                return Result.Ok(Default());
            }

            var ratingRangeValidation = RatingRange.Create(filter.MinRating, filter.MaxRating);

            if (ratingRangeValidation.IsOk)
            {
                var queryFilter = By().Rating(ratingRangeValidation.Value);

                return Result.Ok(queryFilter);
            }

            return Result.Fail<QueryFilter>(ratingRangeValidation.Error);
        }
    }
}
