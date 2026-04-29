using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogDTOService _auditLogService;

        public AuditLogController(IAuditLogDTOService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDTO>>> GetAllAsync([FromQuery] DateOnly? date, [FromQuery] string? shift)
        {
            var auditLogs = await _auditLogService.GetAllAsync(date, shift);

            return Ok(auditLogs);
        }
    }
}
