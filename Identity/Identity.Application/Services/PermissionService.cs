using Identity.Application.Common;
using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces;
using Identity.Domain.Enum;

namespace Identity.Application.Services
{
    public class PermissionService
    {
        private readonly IPermissionService _permissionService;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUser;

        public PermissionService(IPermissionService permissionService, IAuditLogService auditLogService, ICurrentUserService currentUser)
        {
            _permissionService = permissionService;
            _auditLogService = auditLogService;
            _currentUser = currentUser;
        }

        public IReadOnlyList<string> GetCatalog() => Permissions.All;

        public Task<IList<string>> GetRolePermissionsAsync(Guid roleId) =>
            _permissionService.GetRolePermissionsAsync(roleId);

        public async Task AssignToRoleAsync(Guid roleId, AssignPermissionsRequest request)
        {
            await _permissionService.AssignPermissionsToRoleAsync(roleId, request.Permissions);
            await _auditLogService.LogAsync(AuditAction.PermissionsChanged, _currentUser.UserId, _currentUser.IpAddress, _currentUser.UserAgent, $"RoleId={roleId}");
        }

        public Task<IList<string>> GetUserPermissionsAsync(Guid userId) =>
            _permissionService.GetEffectivePermissionsForUserAsync(userId);

        public async Task AssignToUserAsync(Guid userId, AssignPermissionsRequest request)
        {
            await _permissionService.AssignPermissionsToUserAsync(userId, request.Permissions);
            await _auditLogService.LogAsync(AuditAction.PermissionsChanged, _currentUser.UserId, _currentUser.IpAddress, _currentUser.UserAgent, $"UserId={userId}");
        }
    }
}
