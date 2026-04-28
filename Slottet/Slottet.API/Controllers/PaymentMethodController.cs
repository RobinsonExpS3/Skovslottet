using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : Controller
    {
        private readonly IPaymentMethodDTOService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodDTOService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetAllAsync()
        {
            var paymentMethods = await _paymentMethodService.GetAllAsync();

            return Ok(paymentMethods);
        }
    }
}
