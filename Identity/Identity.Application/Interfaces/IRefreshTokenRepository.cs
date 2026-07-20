using Identity.Domain.Entity;

namespace Identity.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshTokens token);
        Task<RefreshTokens?> GetByTokenAsync(string token);
        Task<IList<RefreshTokens>> GetActiveByUserIdAsync(Guid userId);
        Task RevokeAsync(RefreshTokens token);
        Task RevokeAllForUserAsync(Guid userId);
    }
}
