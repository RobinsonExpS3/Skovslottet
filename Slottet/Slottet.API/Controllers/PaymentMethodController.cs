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

        /// <summary>
        /// Gets all payment methods as lookup DTO objects.
        /// </summary>
        /// <returns>Returns all payment methods.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetAllPaymentMethodsAsync()
        {
            var paymentMethods = await _paymentMethodService.GetAllPaymentMethodsAsync();

            return Ok(paymentMethods);
        }
    }
}
