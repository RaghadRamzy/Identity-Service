using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Identity.infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _dbContext;

        public RefreshTokenRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshTokens token)
        {
            await _dbContext.RefreshTokenss.AddAsync(token);
            await _dbContext.SaveChangesAsync();
        }

        public Task<RefreshTokens?> GetByTokenAsync(string token) =>
            _dbContext.RefreshTokenss.FirstOrDefaultAsync(rt => rt.Token == token);

        public async Task<IList<RefreshTokens>> GetActiveByUserIdAsync(Guid userId) =>
            await _dbContext.RefreshTokenss
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpirationDate > DateTime.UtcNow)
                .ToListAsync();

        public async Task RevokeAsync(RefreshTokens token)
        {
            token.IsRevoked = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var tokens = await GetActiveByUserIdAsync(userId);
            foreach (var token in tokens)
                token.IsRevoked = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
