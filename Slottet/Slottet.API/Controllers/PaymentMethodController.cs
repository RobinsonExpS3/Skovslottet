using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Shared;

namespace Slottet.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : Controller {
        private readonly IBaseRepository<PaymentMethod> _repository;

        public PaymentMethodController(IBaseRepository<PaymentMethod> repository) {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodDTO>>> GetAll() {
            var methods = await _repository.GetAllAsync();
            return Ok(methods.Select(m => new PaymentMethodDTO {
                PaymentMethodID = m.PaymentMethodID,
                PaymentMethodName = m.PaymentMethodName
            }));
        }
    }
}