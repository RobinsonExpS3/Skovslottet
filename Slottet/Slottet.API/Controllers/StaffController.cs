using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;


namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : Controller
    {
        private readonly IStaffDTOService _staffService;

        public StaffController(IStaffDTOService staffService)
        {
            _staffService = staffService;
        }

        //Get: Staffs
        [HttpGet("Staffs")]
        public async Task<ActionResult<IEnumerable<EditStaffDTO>>> GetAllAsync()
        {
            var staffs = await _staffService.GetAllAsync();

            return Ok(staffs);
        }

        //Get: Staff by id
        [HttpGet("{id}", Name = "GetStaffById")]
        public async Task<ActionResult<EditStaffDTO>> GetByIdAsync(Guid id)
        {
            var staff = await _staffService.GetByIdAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        //Post: Staff
        [HttpPost]
        public async Task<ActionResult<EditStaffDTO>> CreateAsync([FromBody] EditStaffDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.StaffName) ||
                string.IsNullOrWhiteSpace(dto.Initials) ||
                string.IsNullOrWhiteSpace(dto.Role) ||
                dto.DepartmentID == Guid.Empty)
            {
                return BadRequest("StaffName, Initials, Role, and Department are required.");
            }

            try
            {
                var created = await _staffService.CreateAsync(dto);
                return CreatedAtRoute("GetStaffById", new { id = created.StaffID }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Put: Staff by id
        [HttpPut("{id}")]
        public async Task<ActionResult<EditStaffDTO>> UpdateAsync(Guid id, [FromBody] EditStaffDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.StaffName) ||
                string.IsNullOrWhiteSpace(dto.Initials) ||
                string.IsNullOrWhiteSpace(dto.Role) ||
               dto.DepartmentID == Guid.Empty)
            {
                return BadRequest();
            }

            var updated = await _staffService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        //Delete: Staff by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var existingStaff = await _staffService.DeleteAsync(id);

            if (!existingStaff)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}