using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        [HttpGet("me")]
        public ActionResult<object> GetCurrentUser()
        {
            var roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToArray();

            return Ok(new
            {
                name = User.Identity?.Name,
                roles
            });
        }
    }
}
