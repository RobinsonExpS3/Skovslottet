using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly SlottetDBContext _context;

        public DepartmentController(SlottetDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentLookupDTO>>> GetAllAsync()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.DepartmentName)
                .Select(d => new DepartmentLookupDTO
                {
                    ID = d.DepartmentID,
                    Name = d.DepartmentName
                })
                .ToListAsync();

            return Ok(departments);
        }
    }
}
