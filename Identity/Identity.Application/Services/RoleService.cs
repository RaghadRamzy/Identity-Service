using Identity.Application.Common;
using Identity.Application.DTOs.Roles;
using Identity.Application.Interfaces;
using Identity.Domain.Enum;

namespace Identity.Application.Services
{
   
    public class RoleService
    {
        private readonly IIdentityService _identityService;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUser;

        public RoleService(IIdentityService identityService, IAuditLogService auditLogService, ICurrentUserService currentUser)
        {
            _identityService = identityService;
            _auditLogService = auditLogService;
            _currentUser = currentUser;
        }

        public Task<IList<RoleDto>> GetRolesAsync() => _identityService.GetRolesAsync();

        public Task<RoleDto?> GetRoleByIdAsync(Guid id) => _identityService.GetRoleByIdAsync(id);

        public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequest request)
        {
            var result = await _identityService.CreateRoleAsync(request.Name, request.Description);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.RoleCreated, _currentUser.UserId, _currentUser.IpAddress, _currentUser.UserAgent, request.Name);
            return result;
        }

        public async Task<Result> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
        {
            var result = await _identityService.UpdateRoleAsync(id, request.Name, request.Description);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.RoleUpdated, _currentUser.UserId, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }

        public async Task<Result> DeleteRoleAsync(Guid id)
        {
            var result = await _identityService.DeleteRoleAsync(id);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.RoleDeleted, _currentUser.UserId, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }
    }
}
