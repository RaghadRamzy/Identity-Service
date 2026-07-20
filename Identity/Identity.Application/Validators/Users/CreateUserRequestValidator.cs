using FluentValidation;
using Identity.Application.DTOs.Users;

namespace Identity.Application.Validators.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(3, 50)
                .Matches("^[a-zA-Z0-9_.-]+$").WithMessage("Username may only contain letters, numbers, '.', '_' and '-'.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.TemporaryPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be specified.");

            RuleForEach(x => x.Roles)
                .NotEmpty().WithMessage("Role name cannot be blank.");
        }
    }
}
