using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffPNController : ControllerBase
    {
        private readonly IStaffPNDTOService _staffPNService;

        public StaffPNController(IStaffPNDTOService staffPNService)
        {
            _staffPNService = staffPNService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffPNDTO>>> GetAllAsync()
        {
            var staffPNs = await _staffPNService.GetAllAsync();
            return Ok(staffPNs);
        }

        public async Task<IActionResult> StaffGivesPN([FromBody] StaffPNDTO dto)
        {

            var updated = await _staffPNService.UpdateAsync(dto);

            if (!updated) return NotFound();

            return NoContent();
        }
    }
}
