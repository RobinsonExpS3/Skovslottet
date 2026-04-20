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
    public class PaymentMethodController : Controller {
        private readonly SlottetDBContext _context;

        public PaymentMethodController(SlottetDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodDTO>>> GetAll() {
            var methods = await _context.PaymentMethods
                .AsNoTracking()
                .Select(m => new PaymentMethodDTO {
                    PaymentMethodID = m.PaymentMethodID,
                    PaymentMethodName = m.PaymentMethodName
                })
                .ToListAsync();

            return Ok(methods);
        }
    }
}
