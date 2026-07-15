using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.infrastructure.Services
{
    public class ClientValidator : IClientValidator
    {
        private readonly IConfiguration _configuration;

        public ClientValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValid(string? clientId, string? clientSecret)
        {
            var configuredId = _configuration["OAuthClient:ClientId"];
            var configuredSecret = _configuration["OAuthClient:ClientSecret"];

            if (string.IsNullOrEmpty(configuredId))
                return true;

            return clientId == configuredId && clientSecret == configuredSecret;
        }
    }
}
