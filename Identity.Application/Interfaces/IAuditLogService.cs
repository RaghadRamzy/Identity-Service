using Identity.Domain.Enum;

namespace Identity.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(AuditAction action, Guid? userId, string? ipAddress = null, string? userAgent = null, string? details = null);
    }
}
