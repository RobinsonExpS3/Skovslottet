using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroceryDayController : Controller
    {
        private readonly IGroceryDayDTOService _groceryDayService;

        public GroceryDayController(IGroceryDayDTOService groceryDayService)
        {
            _groceryDayService = groceryDayService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetAllAsync()
        {
            var groceryDays = await _groceryDayService.GetAllAsync();

            return Ok(groceryDays);
        }
    }
}
