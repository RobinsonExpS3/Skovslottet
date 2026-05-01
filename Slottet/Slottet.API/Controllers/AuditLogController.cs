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

        /// <summary>
        /// Gets audit logs filtered by date and shift.
        /// </summary>
        /// <param name="date">The date to filter audit logs by, or null to include all dates.</param>
        /// <param name="shift">The shift name to filter audit logs by, or null to include all shifts.</param>
        /// <returns>Returns the matching audit logs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDTO>>> GetAllAuditLogsAsync([FromQuery] DateOnly? date, [FromQuery] string? shift)
        {
            var auditLogs = await _auditLogService.GetAllAuditLogsAsync(date, shift);

            return Ok(auditLogs);
        }
    }
}
