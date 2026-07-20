using FluentValidation.TestHelper;
using Identity.Application.DTOs.Auth;
using Identity.Application.Validators.Auth;
using Xunit;

namespace Identity.Tests.Validators.Auth
{
    public class ChangePasswordRequestValidatorTests
    {
        private readonly ChangePasswordRequestValidator _validator = new();

        [Fact]
        public void Valid_request_has_no_errors()
        {
            var request = new ChangePasswordRequest { CurrentPassword = "Old!Pass1", NewPassword = "New!Pass2" };

            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void New_password_same_as_current_is_rejected()
        {
            var request = new ChangePasswordRequest { CurrentPassword = "Same!Pass1", NewPassword = "Same!Pass1" };

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Fact]
        public void Weak_new_password_is_rejected()
        {
            var request = new ChangePasswordRequest { CurrentPassword = "Old!Pass1", NewPassword = "weak" };

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }
    }
}
