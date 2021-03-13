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
        public bool IsEmpty { get; private set; }
        public bool FilterByRating { get; private set; }
        public RatingRange RatingRange { get; private set; }

        public static QueryFilter Empty()
        {
            return new()
            {
                IsEmpty = true
            };
        }

        public static QueryFilter By()
        {
            return new()
            {
                IsEmpty = true
            };
        }

        public QueryFilter Rating(RatingRange ratingRange)
        {
            IsEmpty = false;
            FilterByRating = true;
            RatingRange = ratingRange;

            return this;
        }

        public static Result<QueryFilter> CreateFrom(RawFilter filter)
        {
            if (filter.IsEmpty())
            {
                return Result.Ok(Empty());
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
