using FluentValidation.TestHelper;
using Identity.Application.DTOs.Users;
using Identity.Application.Validators.Users;
using Xunit;

namespace Identity.Tests.Validators.Users
{
    public class UpdateUserRolesRequestValidatorTests
    {
        private readonly UpdateUserRolesRequestValidator _validator = new();

        [Fact]
        public void Valid_request_has_no_errors()
        {
            var result = _validator.TestValidate(new UpdateUserRolesRequest { Roles = new List<string> { "Admin" } });
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Empty_roles_is_rejected()
        {
            var result = _validator.TestValidate(new UpdateUserRolesRequest { Roles = new List<string>() });
            result.ShouldHaveValidationErrorFor(x => x.Roles);
        }
    }
}
