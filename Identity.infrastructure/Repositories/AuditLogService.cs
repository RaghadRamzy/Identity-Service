using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Domain.Enum;
using Identity.infrastructure.Data.Context;

namespace Identity.infrastructure.Repositories
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IdentityDbContext _dbContext;

        public AuditLogService(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogAsync(AuditAction action, Guid? userId, string? ipAddress = null, string? userAgent = null, string? details = null)
        {
            _dbContext.AuditLogs.Add(new AuditLog
            {
                Action = action,
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Details = details
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
