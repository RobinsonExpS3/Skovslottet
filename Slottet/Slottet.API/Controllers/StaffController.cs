
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Slottet.Application.Interfaces;
    using Slottet.Domain.Entities;
    using Slottet.Infrastructure.Data;
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
            public async Task<ActionResult<IEnumerable<StaffDTO>>> GetAllAsync()
            {
                var staffs = await _staffService.GetAllAsync();
                
                return Ok(staffs);
            }

            //Get: Staff by id
            [HttpGet("{id}")]
            public async Task<ActionResult<StaffDTO>> GetByIdAsync(Guid id)
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
            public async Task<ActionResult<StaffDTO>> CreateAsync([FromBody] StaffDTO dto)
            {
                if (dto == null ||
                    string.IsNullOrWhiteSpace(dto.StaffName) ||
                    string.IsNullOrWhiteSpace(dto.Initials) ||
                    string.IsNullOrWhiteSpace(dto.Role))
                {
                    return BadRequest();
                }

                var result = await _staffService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetByIdAsync), new { id = dto.StaffID }, result);
            }

            //Put: Staff by id
            [HttpPut("{id}")]
            public async Task<ActionResult<StaffDTO>> UpdateAsync(Guid id, [FromBody] StaffDTO dto)
            {
                if (dto == null ||
                    string.IsNullOrWhiteSpace(dto.StaffName) ||
                    string.IsNullOrWhiteSpace(dto.Initials) ||
                    string.IsNullOrWhiteSpace(dto.Role))
                {
                    return BadRequest();
                }

                var updated = await _staffService.UpdateAsync(id, dto);
                
                if(!updated) {
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
