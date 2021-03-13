using System;

namespace PlayerFeedbackService.Domain
{
    public record Result
    {
        public bool IsOk { get; }
        public Error Error { get; }
        public bool IsError => !IsOk;

        protected Result()
        {
            IsOk = true;
        }

        protected Result(Error error)
        {
            if (error == null)
            {
                throw new InvalidOperationException("Result.Error cannot be null");
            }

            IsOk = false;
            Error = error;
        }

        public static Result Ok()
        {
            return new();
        }

        public static Result<T> Ok<T>(T value)
        {
            return new(value);
        }

        public static Result Fail(Error error)
        {
            return new(error);
        }

        public static Result<T> Fail<T>(Error error)
        {
            return new(error);
        }
    }

    public record Result<T> : Result
    {
        public T Value { get; }

        protected internal Result(T value)
        {
            Value = value;
        }

        protected internal Result(Error error) : base(error) {}
    }
}
