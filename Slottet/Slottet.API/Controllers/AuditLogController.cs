using Microsoft.AspNetCore.Mvc;
using Slottet.Infrastructure.Data;
using Slottet.Domain.Entities;
using Slottet.Shared;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;

namespace Slottet.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : Controller {
        private readonly SlottetDBContext _context;
        private readonly IAuditLogDTOService _auditLogService;

        public AuditLogController(IAuditLogDTOService auditLogService) {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDTO>>> GetAllAsync([FromQuery] DateOnly? date, [FromQuery] string? shift) {
            IQueryable<AuditLog> query = _context.AuditLogs.AsNoTracking();

            if(date.HasValue) {
                var from = date.Value.ToDateTime(TimeOnly.MinValue);
                var to = from.AddDays(1);

                query = query.Where(log => 
                    (log.PerformedAtTime ?? log.TimeStamp) >= from && 
                    (log.PerformedAtTime ?? log.TimeStamp) < to);
            }

            if (!string.IsNullOrWhiteSpace(shift)) {
                var normalizedShift = shift.Trim().ToLowerInvariant();

                query = normalizedShift switch {
                    "day" => query.Where(log =>
                        (log.PerformedAtTime ?? log.TimeStamp).Hour >= 7 &&
                        (log.PerformedAtTime ?? log.TimeStamp).Hour < 15),
                    "evening" => query.Where(log =>
                        (log.PerformedAtTime ?? log.TimeStamp).Hour >= 15 &&
                        (log.PerformedAtTime ?? log.TimeStamp).Hour < 23),
                    "night" => query.Where(log =>
                        (log.PerformedAtTime ?? log.TimeStamp).Hour >= 23 ||
                        (log.PerformedAtTime ?? log.TimeStamp).Hour < 7),
                    _ => query
                };
            }

            var result = await _auditLogService.GetAllAsync();

            return Ok(result);
        }
    }
}
