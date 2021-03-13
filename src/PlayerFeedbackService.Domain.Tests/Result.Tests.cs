using Xunit;

namespace PlayerFeedbackService.Domain.Tests
{
    public class ResultTests
    {
        public class ResultOkShould
        {
            [Fact]
            public void Set_IsOkTrue_And_IsErrorFalse()
            {
                var result = Result.Ok();

                Assert.True(result.IsOk);
                Assert.False(result.IsError);
            }

            [Fact]
            public void Set_Value()
            {
                var value = 1;
                var result = Result.Ok(value);

                Assert.True(result.IsOk);
                Assert.Equal(value, result.Value);
            }
        }

        public class ResultFailShould
        {
            [Fact]
            public void Set_IsOkFalse_And_IsErrorTrue()
            {
                var error = new Error
                {
                    Type = "Error Type",
                    Message = "Error Message"
                };

                var result = Result.Fail(error);

                Assert.False(result.IsOk);
                Assert.True(result.IsError);
            }

            [Fact]
            public void Set_Error()
            {
                var error = new Error
                {
                    Type = "Error Type",
                    Message = "Error Message"
                };

                var result = Result.Fail(error);

                Assert.Equal(error, result.Error);
            }
        }
    }
}
