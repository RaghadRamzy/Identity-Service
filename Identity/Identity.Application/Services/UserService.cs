using Identity.Application.Common;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces;
using Identity.Domain.Enum;

namespace Identity.Application.Services
{
    
    public class UserService
    {
        private readonly IIdentityService _identityService;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUser;

        public UserService(IIdentityService identityService, IAuditLogService auditLogService, ICurrentUserService currentUser)
        {
            _identityService = identityService;
            _auditLogService = auditLogService;
            _currentUser = currentUser;
        }

        public Task<IList<UserDto>> GetUsersAsync() => _identityService.GetUsersAsync();

        public Task<UserDto?> GetUserByIdAsync(Guid id) => _identityService.GetUserByIdAsync(id);

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request)
        {
            var result = await _identityService.CreateUserAsync(
                request.Username, request.FullName, request.Email, request.PhoneNumber, request.TemporaryPassword, request.Roles);

            if (result.Succeeded && result.Data is not null)
                await _auditLogService.LogAsync(AuditAction.UserCreated, result.Data.Id, _currentUser.IpAddress, _currentUser.UserAgent);

            return result;
        }

        public async Task<Result> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            var result = await _identityService.UpdateUserAsync(id, request.FullName, request.PhoneNumber);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.UserUpdated, id, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            var result = await _identityService.DeleteUserAsync(id);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.UserDeleted, id, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }

        public async Task<Result> UpdateUserRolesAsync(Guid id, IEnumerable<string> roles)
        {
            var result = await _identityService.UpdateUserRolesAsync(id, roles);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.UserRolesUpdated, id, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }

        public Task<Result> ActivateAsync(Guid id) => _identityService.SetActiveAsync(id, true);

        public Task<Result> DeactivateAsync(Guid id) => _identityService.SetActiveAsync(id, false);

        public async Task<Result> LockAsync(Guid id, TimeSpan? duration = null)
        {
            var result = await _identityService.LockUserAsync(id, duration ?? TimeSpan.FromMinutes(15));
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.AccountLocked, id, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }

        public async Task<Result> UnlockAsync(Guid id)
        {
            var result = await _identityService.UnlockUserAsync(id);
            if (result.Succeeded)
                await _auditLogService.LogAsync(AuditAction.AccountUnlocked, id, _currentUser.IpAddress, _currentUser.UserAgent);
            return result;
        }
    }
}
