using FluentValidation.TestHelper;
using Identity.Application.DTOs.Auth;
using Identity.Application.Validators.Auth;
using Xunit;

namespace Identity.Tests.Validators.Auth
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator = new();

        [Fact]
        public void Valid_request_has_no_errors()
        {
            var result = _validator.TestValidate(new LoginRequest { UsernameOrEmail = "raghad", Password = "whatever" });
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Empty_username_is_rejected()
        {
            var result = _validator.TestValidate(new LoginRequest { UsernameOrEmail = "", Password = "whatever" });
            result.ShouldHaveValidationErrorFor(x => x.UsernameOrEmail);
        }

        [Fact]
        public void Empty_password_is_rejected()
        {
            var result = _validator.TestValidate(new LoginRequest { UsernameOrEmail = "raghad", Password = "" });
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
