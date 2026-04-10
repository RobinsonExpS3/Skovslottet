using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ShiftboardController : Controller
    {
        private readonly IBaseRepository<ShiftBoard> _repository;
    }
}
