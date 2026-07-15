using Identity.Application.DTOs.Users;

namespace Identity.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
