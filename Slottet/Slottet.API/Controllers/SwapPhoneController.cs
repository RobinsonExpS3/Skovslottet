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
        public async Task<ActionResult<IEnumerable<SwapPhoneRecDTO>>> GetAll()
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
        public async Task<IActionResult> SwapPhone([FromBody] SwapPhoneRecDTO dto)
        {
            //if (dto == null ||  dto.StaffID == Guid.Empty) return BadRequest();

            var updated = await _swapPhoneService.UpdateAsync(dto);
            if (!updated) return NotFound();

            return NoContent();
        }
    }
}
