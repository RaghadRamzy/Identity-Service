using Identity.Application.DTOs.Roles;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    
    [ApiController]
    [Route("api/roles")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RolesController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _roleService.GetRolesAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return role is null ? NotFound() : Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleRequest request)
        {
            var result = await _roleService.CreateRoleAsync(request);
            return result.Succeeded
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
                : BadRequest(result.Errors);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateRoleRequest request)
        {
            var result = await _roleService.UpdateRoleAsync(id, request);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }
    }
}
