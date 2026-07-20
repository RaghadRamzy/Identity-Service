using Identity.Application.DTOs.Users;

namespace Identity.Application.Interfaces
{
    public interface ITokenService
    {
        (string token, DateTime expiration) GenerateAccessToken(UserDto user);
        string GenerateRefreshToken();
        Guid? GetUserIdFromExpiredToken(string accessToken);
    }
}
