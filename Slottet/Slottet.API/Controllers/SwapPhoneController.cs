using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SwapPhoneController : ControllerBase
    {
        private readonly ISwapPhoneDTOService _swapPhoneService;

        public SwapPhoneController(ISwapPhoneDTOService swapPhoneService)
        {
            _swapPhoneService = swapPhoneService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwapPhoneDTO>>> GetAll()
        {
            return Ok(await _swapPhoneService.GetAllAsync());
        }

        //[HttpGet("{phoneId:guid}")]
        //public async Task<ActionResult<SwapPhoneDTO>> GetById(Guid phoneId)
        //{
        //    var phone = await _swapPhoneService.GetByIdAsync(phoneId);

        //    if (phone == null) return NotFound();

        //    return Ok(phone);
        //}

        [HttpPost("assign")]
        public async Task<IActionResult> SwapPhone(Guid phoneId, Guid staffId, [FromBody] SwapPhoneDTO dto)
        {
            //if (dto == null ||  dto.StaffID == Guid.Empty) return BadRequest();

            var updated = await _swapPhoneService.UpdateAsync(phoneId, staffId, dto);
            if (!updated) return NotFound();

            return NoContent();
        }
    }
}
