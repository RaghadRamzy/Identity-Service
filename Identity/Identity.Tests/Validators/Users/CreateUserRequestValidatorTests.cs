using FluentValidation.TestHelper;
using Identity.Application.DTOs.Users;
using Identity.Application.Validators.Users;
using Xunit;

namespace Identity.Tests.Validators.Users
{
    public class CreateUserRequestValidatorTests
    {
        private readonly CreateUserRequestValidator _validator = new();

        private static CreateUserRequest ValidRequest() => new()
        {
            Username = "new_user",
            FullName = "New User",
            Email = "newuser@example.com",
            PhoneNumber = "01098765432",
            TemporaryPassword = "Temp0rary!",
            Roles = new List<string> { "Customer" }
        };

        [Fact]
        public void Valid_request_has_no_errors()
        {
            var result = _validator.TestValidate(ValidRequest());
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Empty_roles_is_rejected()
        {
            var request = ValidRequest();
            request.Roles = new List<string>();

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Roles);
        }

        [Fact]
        public void Blank_role_entry_is_rejected()
        {
            var request = ValidRequest();
            request.Roles = new List<string> { "" };

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor("Roles[0]");
        }

        [Fact]
        public void Weak_temporary_password_is_rejected()
        {
            var request = ValidRequest();
            request.TemporaryPassword = "weak";

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.TemporaryPassword);
        }
    }
}
