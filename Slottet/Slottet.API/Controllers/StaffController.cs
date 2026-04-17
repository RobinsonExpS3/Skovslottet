using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : Controller {
        private readonly IBaseRepository<Staff> _repository;

        public StaffController(IBaseRepository<Staff> repository) {
            _repository = repository;
        }

        //Get: Staffs
        [HttpGet("Staffs")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetAll() {
            var staffs = await _repository.GetAllAsync();
            return Ok(staffs);
        }

        //Get: Staff by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetById(Guid id) {
            var staff = await _repository.GetByIdAsync(id);

            if (staff == null) {
                return NotFound();
            }

            return Ok(staff);
        }

        //Post: Staff
        [HttpPost]
        public async Task<ActionResult<Staff>> CreateStaff([FromBody] Staff staff) {
            if (staff == null ||
                string.IsNullOrWhiteSpace(staff.StaffName) ||
                string.IsNullOrWhiteSpace(staff.Initials) ||
                string.IsNullOrWhiteSpace(staff.Role)) {
                return BadRequest();
            }

            staff.StaffID = Guid.NewGuid();

            await _repository.AddAsync(staff);

            return CreatedAtAction(nameof(GetById), new { id = staff.StaffID }, staff);
        }

        //Put: Staff by id
        [HttpPut("{id}")]
        public async Task<ActionResult<Staff>> UpdateStaff(Guid id, [FromBody] Staff staff) {
            if (staff == null ||
                string.IsNullOrWhiteSpace(staff.StaffName) ||
                string.IsNullOrWhiteSpace(staff.Initials) ||
                string.IsNullOrWhiteSpace(staff.Role)) {
                return BadRequest();
            }

            var existingStaff = await _repository.GetByIdAsync(id);

            if (existingStaff == null) {
                return NotFound();
            }

            existingStaff.StaffName = staff.StaffName;
            existingStaff.Initials = staff.Initials;
            existingStaff.Role = staff.Role;
            existingStaff.DepartmentID = staff.DepartmentID;

            await _repository.UpdateAsync(existingStaff);

            return NoContent();
        }

        //Delete: Staff by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaff(Guid id) {
            var existingStaff = await _repository.GetByIdAsync(id);

            if (existingStaff == null) {
                return NotFound();
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
