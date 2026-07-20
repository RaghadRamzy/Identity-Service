using Identity.Application.Common;
using Identity.Application.DTOs.Roles;
using Identity.Application.DTOs.Users;

namespace Identity.Application.Interfaces
{

    public interface IIdentityService
    {
        Task<Result<UserDto>> CreateUserAsync(string username, string fullName, string email, string phoneNumber, string password, IEnumerable<string> roles);  
        Task<Result<UserDto>> ValidateCredentialsAsync(string usernameOrEmail, string password);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IList<UserDto>> GetUsersAsync();
        Task<Result> UpdateUserAsync(Guid userId, string? fullName, string? phoneNumber);
        Task<Result> UpdateUserRolesAsync(Guid userId, IEnumerable<string> roles);
        Task<Result> DeleteUserAsync(Guid userId);
        Task<Result> SetActiveAsync(Guid userId, bool isActive);
        Task<Result> LockUserAsync(Guid userId, TimeSpan? duration);
        Task<Result> UnlockUserAsync(Guid userId);
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> IsLockedOutAsync(Guid userId);

      
        Task<Result<RoleDto>> CreateRoleAsync(string name, string? description);
        Task<Result> UpdateRoleAsync(Guid roleId, string? name, string? description);
        Task<Result> DeleteRoleAsync(Guid roleId);
        Task<IList<RoleDto>> GetRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(Guid roleId);
    }
}
