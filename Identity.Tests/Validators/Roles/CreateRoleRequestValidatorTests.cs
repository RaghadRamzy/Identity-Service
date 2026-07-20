using FluentValidation.TestHelper;
using Identity.Application.DTOs.Roles;
using Identity.Application.Validators.Roles;
using Xunit;

namespace Identity.Tests.Validators.Roles
{
    public class CreateRoleRequestValidatorTests
    {
        private readonly CreateRoleRequestValidator _validator = new();

        [Fact]
        public void Valid_request_has_no_errors()
        {
            var result = _validator.TestValidate(new CreateRoleRequest { Name = "Manager", Description = "Manages things" });
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Too_short_name_is_rejected()
        {
            var result = _validator.TestValidate(new CreateRoleRequest { Name = "A" });
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Too_long_description_is_rejected()
        {
            var result = _validator.TestValidate(new CreateRoleRequest { Name = "Manager", Description = new string('x', 201) });
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}
