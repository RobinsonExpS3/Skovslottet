using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Slottet.API.Controllers
{
    
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MicLogin : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hey! Det virker!");
        }
    }
}
