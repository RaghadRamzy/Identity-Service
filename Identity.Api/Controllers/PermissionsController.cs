using Identity.Application.DTOs.Permissions;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
   
    [ApiController]
    [Route("api/permissions")]
    [Authorize(Roles = "Admin")]
    public class PermissionsController : ControllerBase
    {
        private readonly PermissionService _permissionService;

        public PermissionsController(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("catalog")]
        public IActionResult GetCatalog() => Ok(_permissionService.GetCatalog());

        [HttpGet("roles/{roleId:guid}")]
        public async Task<IActionResult> GetRolePermissions(Guid roleId) =>
            Ok(await _permissionService.GetRolePermissionsAsync(roleId));

        [HttpPost("roles/{roleId:guid}/assign")]
        public async Task<IActionResult> AssignToRole(Guid roleId, AssignPermissionsRequest request)
        {
            await _permissionService.AssignToRoleAsync(roleId, request);
            return NoContent();
        }

        [HttpGet("users/{userId:guid}")]
        public async Task<IActionResult> GetUserPermissions(Guid userId) =>
            Ok(await _permissionService.GetUserPermissionsAsync(userId));

        [HttpPost("users/{userId:guid}/assign")]
        public async Task<IActionResult> AssignToUser(Guid userId, AssignPermissionsRequest request)
        {
            await _permissionService.AssignToUserAsync(userId, request);
            return NoContent();
        }
    }
}
