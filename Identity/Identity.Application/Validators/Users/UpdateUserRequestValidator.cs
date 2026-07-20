using FluentValidation;
using Identity.Application.DTOs.Users;

namespace Identity.Application.Validators.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100)
                .When(x => x.FullName is not null);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => x.PhoneNumber is not null);
        }
    }
}
