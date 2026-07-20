using FluentValidation.TestHelper;
using Identity.Application.DTOs.Auth;
using Identity.Application.Validators.Auth;
using Xunit;

namespace Identity.Tests.Validators.Auth
{
    public class RegisterRequestValidatorTests
    {
        private readonly RegisterRequestValidator _validator = new();

        private static RegisterRequest ValidRequest() => new()
        {
            Username = "raghad_dev",
            FullName = "Raghad Example",
            Email = "raghad@example.com",
            PhoneNumber = "01012345678",
            Password = "Str0ng!Pass",
            Roles = new List<string> { "Customer" }
        };

        [Fact]
        public void Valid_request_has_no_errors()
        {
            var result = _validator.TestValidate(ValidRequest());
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("has spaces")]
        public void Invalid_username_is_rejected(string username)
        {
            var request = ValidRequest();
            request.Username = username;

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData("")]
        public void Invalid_email_is_rejected(string email)
        {
            var request = ValidRequest();
            request.Email = email;

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Empty_roles_is_rejected()
        {
            var request = ValidRequest();
            request.Roles = new List<string>();

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Roles);
        }

        [Theory]
        [InlineData("short1!")]      // too short
        [InlineData("alllowercase1!")] // no uppercase
        [InlineData("ALLUPPERCASE1!")] // no lowercase
        [InlineData("NoDigitsHere!")]  // no digit
        [InlineData("NoSymbols123")]   // no non-alphanumeric
        public void Weak_password_is_rejected(string password)
        {
            var request = ValidRequest();
            request.Password = password;

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
