using FluentValidation;
using Identity.Application.DTOs.Roles;

namespace Identity.Application.Validators.Roles
{
    public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(2, 50)
                .When(x => x.Name is not null);

            RuleFor(x => x.Description)
                .MaximumLength(200)
                .When(x => x.Description is not null);
        }
    }
}
