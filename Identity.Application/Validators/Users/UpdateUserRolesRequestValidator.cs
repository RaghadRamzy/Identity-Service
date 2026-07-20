using FluentValidation;
using Identity.Application.DTOs.Users;

namespace Identity.Application.Validators.Users
{
    public class UpdateUserRolesRequestValidator : AbstractValidator<UpdateUserRolesRequest>
    {
        public UpdateUserRolesRequestValidator()
        {
            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be provided.");

            RuleForEach(x => x.Roles)
                .NotEmpty().WithMessage("Role name cannot be blank.");
        }
    }
}
