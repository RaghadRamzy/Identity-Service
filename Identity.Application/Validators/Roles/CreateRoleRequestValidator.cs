using FluentValidation;
using Identity.Application.DTOs.Roles;

namespace Identity.Application.Validators.Roles
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 50);

            RuleFor(x => x.Description)
                .MaximumLength(200)
                .When(x => x.Description is not null);
        }
    }
}
