using Identity.Application.Common;
using Identity.Application.DTOs.Roles;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces;
using Identity.Domain.Entity.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.infrastructure.Services
{
    public class IdentityServiceImpl : IIdentityService
    {
        private const string PermissionClaimType = "permission";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityServiceImpl(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<UserDto>> CreateUserAsync(string username, string fullName, string email, string phoneNumber, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                PhoneNumber = phoneNumber,
                FullName = fullName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, password); 
            if (!createResult.Succeeded)
                return Result<UserDto>.Failure(createResult.Errors.Select(e => e.Description).ToArray());

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });

            await _userManager.AddToRoleAsync(user, role);

            return Result<UserDto>.Success(await ToDtoAsync(user));
        }

        public async Task<Result<UserDto>> ValidateCredentialsAsync(string usernameOrEmail, string password)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user is null)
                return Result<UserDto>.Failure("Invalid username or password.");

            if (await _userManager.IsLockedOutAsync(user))
                return Result<UserDto>.Failure("Account is locked. Try again later.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(user);
                return Result<UserDto>.Failure("Invalid username or password.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            return Result<UserDto>.Success(await ToDtoAsync(user));
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user is null ? null : await ToDtoAsync(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is null ? null : await ToDtoAsync(user);
        }

        public async Task<IList<UserDto>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDto>();
            foreach (var user in users)
                result.Add(await ToDtoAsync(user));
            return result;
        }

        public async Task<Result> UpdateUserAsync(Guid userId, string? fullName, string? phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure("User not found.");

            if (!string.IsNullOrWhiteSpace(fullName)) user.FullName = fullName;
            if (!string.IsNullOrWhiteSpace(phoneNumber)) user.PhoneNumber = phoneNumber;
            user.UpdatedDate = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure("User not found.");

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> SetActiveAsync(Guid userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure("User not found.");

            user.IsActive = isActive;
            user.UpdatedDate = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> LockUserAsync(Guid userId, TimeSpan? duration)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure("User not found.");

            await _userManager.SetLockoutEnabledAsync(user, true);
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(duration ?? TimeSpan.FromMinutes(15)));
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> UnlockUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure("User not found.");

            await _userManager.ResetAccessFailedCountAsync(user);
            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Failure("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user is null ? string.Empty : await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return Result.Failure("Invalid request.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<bool> IsLockedOutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user is not null && await _userManager.IsLockedOutAsync(user);
        }

      

        public async Task<Result<RoleDto>> CreateRoleAsync(string name, string? description)
        {
            if (await _roleManager.RoleExistsAsync(name))
                return Result<RoleDto>.Failure("Role already exists.");

            var role = new ApplicationRole { Name = name, Description = description };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return Result<RoleDto>.Failure(result.Errors.Select(e => e.Description).ToArray());

            return Result<RoleDto>.Success(new RoleDto { Id = role.Id, Name = role.Name!, Description = role.Description });
        }

        public async Task<Result> UpdateRoleAsync(Guid roleId, string? name, string? description)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return Result.Failure("Role not found.");

            if (!string.IsNullOrWhiteSpace(name)) role.Name = name;
            role.Description = description ?? role.Description;

            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> DeleteRoleAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return Result.Failure("Role not found.");

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<IList<RoleDto>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var result = new List<RoleDto>();
            foreach (var role in roles)
                result.Add(await BuildRoleDtoAsync(role));
            return result;
        }

        public async Task<RoleDto?> GetRoleByIdAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            return role is null ? null : await BuildRoleDtoAsync(role);
        }

        private async Task<RoleDto> BuildRoleDtoAsync(ApplicationRole role)
        {
           
            var claims = await _roleManager.GetClaimsAsync(role);
            var permissionNames = claims
                .Where(c => c.Type == PermissionClaimType)
                .Select(c => c.Value)
                .ToList();

            return new RoleDto { Id = role.Id, Name = role.Name!, Description = role.Description, Permissions = permissionNames };
        }

        private async Task<UserDto> ToDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                IsLocked = await _userManager.IsLockedOutAsync(user),
                CreatedDate = user.CreatedDate,
                Roles = roles
            };
        }
    }
}
