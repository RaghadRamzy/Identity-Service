using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetServices()
        {
            return Ok("Authenticated user.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateService()
        {
            return Ok("Admin only.");
        }
    }
}
