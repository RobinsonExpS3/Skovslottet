using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
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
            private readonly IBaseRepository<Staff> _repository;

            public StaffController(SlottetDBContext context)
            {
                _context = context;
            }

            //Get: Staffs
            [HttpGet("Staffs")]
            public async Task<ActionResult<IEnumerable<Staff>>> GetAll()
            {

                var staffs = await _context.Staffs
                    .AsNoTracking()
                    .ToListAsync();
                return Ok(staffs);
            }

            //Get: Staff by id
            [HttpGet("{id}")]
            public async Task<ActionResult<Staff>> GetById(Guid id)
            {
                var staff = await _context.Staff
    
            if (staff == null)
                {
                    return NotFound();
                }

                return Ok(staff);
            }

            //Post: Staff
            [HttpPost]
            public async Task<ActionResult<Staff>> CreateStaff([FromBody] Staff staff)
            {
                if (staff == null ||
                    string.IsNullOrWhiteSpace(staff.StaffName) ||
                    string.IsNullOrWhiteSpace(staff.Initials) ||
                    string.IsNullOrWhiteSpace(staff.Role))
                {
                    return BadRequest();
                }

                staff.StaffID = Guid.NewGuid();

                _context.Staffs.Add(staff);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = staff.StaffID }, staff);
            }

            //Put: Staff by id
            [HttpPut("{id}")]
            public async Task<ActionResult<Staff>> UpdateStaff(Guid id, [FromBody] Staff staff)
            {
                if (staff == null ||
                    string.IsNullOrWhiteSpace(staff.StaffName) ||
                    string.IsNullOrWhiteSpace(staff.Initials) ||
                    string.IsNullOrWhiteSpace(staff.Role))
                {
                    return BadRequest();
                }

                var existingStaff = await _context.Staffs.FindAsync(id);

                if (existingStaff == null)
                {
                    return NotFound();
                }

                existingStaff.StaffName = staff.StaffName;
                existingStaff.Initials = staff.Initials;
                existingStaff.Role = staff.Role;
                existingStaff.DepartmentID = staff.DepartmentID;

                await _context.SaveChangesAsync();

                return NoContent();
            }

            //Delete: Staff by id
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteStaff(Guid id)
            {
                var existingStaff = await _context.Staffs.FindAsync(id);

                if (existingStaff == null)
                {
                    return NotFound();
                }

                _context.Staffs.Remove(existingStaff);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
