using FluentValidation;
using Identity.Application.Common;
using Identity.Application.DTOs.Permissions;

namespace Identity.Application.Validators.Permissions
{
    public class AssignPermissionsRequestValidator : AbstractValidator<AssignPermissionsRequest>
    {
        public AssignPermissionsRequestValidator()
        {
            RuleForEach(x => x.Permissions)
                .Must(p => Identity.Application.Common.Permissions.All.Contains(p))
                .WithMessage((_, permission) => $"'{permission}' is not a recognized permission.");
        }
    }
}
