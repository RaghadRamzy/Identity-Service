using FluentValidation;
using Identity.Application.DTOs.Auth;

namespace Identity.Application.Validators.Auth
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        private static readonly string[] SupportedGrantTypes = { "password", "refresh_token" };

        public TokenRequestValidator()
        {
            RuleFor(x => x.GrantType)
                .NotEmpty()
                .Must(g => SupportedGrantTypes.Contains(g.Trim().ToLowerInvariant()))
                .WithMessage($"grant_type must be one of: {string.Join(", ", SupportedGrantTypes)}.");

            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();

            When(x => (x.GrantType ?? string.Empty).Trim().ToLowerInvariant() == "password", () =>
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            });

            When(x => (x.GrantType ?? string.Empty).Trim().ToLowerInvariant() == "refresh_token", () =>
            {
                RuleFor(x => x.RefreshToken).NotEmpty();
            });
        }
    }
}
