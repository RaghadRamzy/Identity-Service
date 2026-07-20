using Identity.Application.Interfaces;
using Identity.Domain.Entity.identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.infrastructure.Services
{
    public class PermissionServiceImpl : IPermissionService
    {
        private const string PermissionClaimType = "permission";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PermissionServiceImpl(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<string>> GetRolePermissionsAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return new List<string>();

            var claims = await _roleManager.GetClaimsAsync(role);
            return claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();
        }

        public async Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return;

            var existing = (await _roleManager.GetClaimsAsync(role))
                .Where(c => c.Type == PermissionClaimType)
                .ToList();

            foreach (var claim in existing)
                await _roleManager.RemoveClaimAsync(role, claim);

            foreach (var permission in permissions.Distinct())
                await _roleManager.AddClaimAsync(role, new Claim(PermissionClaimType, permission));
        }

        public async Task<IList<string>> GetUserDirectPermissionsAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return new List<string>();

            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();
        }

        public async Task AssignPermissionsToUserAsync(Guid userId, IEnumerable<string> permissions)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return;

            var existing = (await _userManager.GetClaimsAsync(user))
                .Where(c => c.Type == PermissionClaimType)
                .ToList();

            foreach (var claim in existing)
                await _userManager.RemoveClaimAsync(user, claim);

            foreach (var permission in permissions.Distinct())
                await _userManager.AddClaimAsync(user, new Claim(PermissionClaimType, permission));
        }

        public async Task<IList<string>> GetEffectivePermissionsForUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return new List<string>();

            var direct = (await _userManager.GetClaimsAsync(user))
                .Where(c => c.Type == PermissionClaimType)
                .Select(c => c.Value);

            var roleNames = await _userManager.GetRolesAsync(user);
            var fromRoles = new List<string>();
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role is null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                fromRoles.AddRange(roleClaims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value));
            }

            return direct.Concat(fromRoles).Distinct().ToList();
        }
    }
}
