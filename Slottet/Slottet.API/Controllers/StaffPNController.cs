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

        /// <summary>
        /// Gets all PN assignments given by staff members.
        /// </summary>
        /// <returns>Returns all staff PN assignments.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffPNDTO>>> GetAllStaffPNsAsync()
        {
            var staffPNs = await _staffPNService.GetAllStaffPNsAsync();
            return Ok(staffPNs);
        }

        /// <summary>
        /// Records that a staff member has given a PN.
        /// </summary>
        /// <param name="dto">DTO object containing PN and staff information.</param>
        /// <returns>Returns NoContent if the assignment succeeds, otherwise NotFound.</returns>
        [HttpPost]
        public async Task<IActionResult> PostStaffPNAsync([FromBody] StaffPNDTO dto)
        {

            var updated = await _staffPNService.PostStaffPNAsync(dto);

            if (!updated) return NotFound();

            return NoContent();
        }
    }
}
