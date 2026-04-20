using Slottet.Application.Interfaces;
using System.Security.Claims;

namespace Slottet.API.Middlewares {
    public class AuditScopeMiddleware {
        private readonly RequestDelegate _next;

        public AuditScopeMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditScope auditScope) {
            auditScope.PerformedAtTime = DateTime.Now;

            if(context.User?.Identity?.IsAuthenticated == true) {
                auditScope.PerformedByStaffName = 
                    context.User.FindFirst("StaffName")?.Value ??
                    context.User.FindFirst(ClaimTypes.Name)?.Value ??
                    context.User.Identity?.Name;

            var staffIDValue =
                    context.User.FindFirst("StaffID")?.Value ??
                    context.User.FindFirst("staff_id")?.Value ??
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            auditScope.PerformedByStaffID =
                    Guid.TryParse(staffIDValue, out var staffId) ? staffId : null;
            } else {
                auditScope.PerformedByStaffName = "Anonymous";
                auditScope.PerformedByStaffID = null;
            }

                await _next(context);
        }
    }
}
