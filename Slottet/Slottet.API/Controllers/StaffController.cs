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

        /// <summary>
        /// Gets all staff as DTO objects.
        /// </summary>
        /// <returns>Returns all staff.</returns>
        [HttpGet("Staffs")]
        public async Task<ActionResult<IEnumerable<EditStaffDTO>>> GetAllStaffAsync()
        {
            var staffs = await _staffService.GetAllStaffAsync();

            return Ok(staffs);
        }

        /// <summary>
        /// Gets a staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member to retrieve.</param>
        /// <returns>Returns the staff member if found, otherwise NotFound.</returns>
        [HttpGet("{id}", Name = "GetStaffById")]
        public async Task<ActionResult<EditStaffDTO>> GetStaffByIdAsync(Guid id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        /// <summary>
        /// Creates a new staff member.
        /// </summary>
        /// <param name="dto">DTO object containing staff information.</param>
        /// <returns>Returns the created staff member.</returns>
        [HttpPost]
        public async Task<ActionResult<EditStaffDTO>> PostStaffAsync([FromBody] EditStaffDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.StaffName) ||
                string.IsNullOrWhiteSpace(dto.Initials) ||
                string.IsNullOrWhiteSpace(dto.Role) ||
                dto.DepartmentID == Guid.Empty)
            {
                return BadRequest("StaffName, Initials, Role, and Department are required.");
            }

            var result = await _staffService.PostStaffAsync(dto);
            if (result == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetStaffByIdAsync), new { id = result.StaffID }, result);
        }

        /// <summary>
        /// Updates a staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member to update.</param>
        /// <param name="dto">DTO object containing updated staff information.</param>
        /// <returns>Returns NoContent if the update succeeds, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EditStaffDTO>> PutStaffAsync(Guid id, [FromBody] EditStaffDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.StaffName) ||
                string.IsNullOrWhiteSpace(dto.Initials) ||
                string.IsNullOrWhiteSpace(dto.Role) ||
               dto.DepartmentID == Guid.Empty)
            {
                return BadRequest();
            }

            var updated = await _staffService.PutStaffAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member to delete.</param>
        /// <returns>Returns NoContent if the deletion succeeds, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaffAsync(Guid id)
        {
            var existingStaff = await _staffService.DeleteStaffAsync(id);

            if (!existingStaff)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
