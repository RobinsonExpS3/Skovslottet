using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthDTOService _authService;

        public AuthController(IAuthDTOService authService)
        {
            _authService = authService;
        }

        [HttpGet("me")]
        [Authorize(Policy = "ShiftboardDisplay")]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateUserForStaffDto>> GetStaffUserAsync(Guid id)
        {
            var result = await _authService.GetStaffUserAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("createUserForStaff")]
        public async Task<ActionResult<CreateUserForStaffDto>> PostStaffUserAsync([FromBody] CreateUserForStaffDto dto)
        {
            var result = await _authService.PostStaffUserAsync(dto);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CreateUserForStaffDto>> PutStaffUserAsync(Guid id, [FromBody] CreateUserForStaffDto dto)
        {
            var updated = await _authService.PutStaffUserAsync(id, dto);

            if (!updated)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaffUser(Guid id)
        {
            var deleted = await _authService.DeleteStaffUserAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}
