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

        /// <summary>
        /// Gets all grocery days as lookup DTO objects.
        /// </summary>
        /// <returns>Returns all grocery days.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetAllGroceryDaysAsync()
        {
            var groceryDays = await _groceryDayService.GetAllGroceryDaysAsync();

            return Ok(groceryDays);
        }
    }
}
