using Identity.Application.DTOs.Users;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _userService.GetUsersAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return result.Succeeded
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
                : BadRequest(result.Errors);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpPut("{id:guid}/roles")]
        public async Task<IActionResult> UpdateRoles(Guid id, UpdateUserRolesRequest request)
        {
            var result = await _userService.UpdateUserRolesAsync(id, request.Roles);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpPost("{id:guid}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var result = await _userService.ActivateAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpPost("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await _userService.DeactivateAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpPost("{id:guid}/lock")]
        public async Task<IActionResult> Lock(Guid id)
        {
            var result = await _userService.LockAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        [HttpPost("{id:guid}/unlock")]
        public async Task<IActionResult> Unlock(Guid id)
        {
            var result = await _userService.UnlockAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }
    }
}
