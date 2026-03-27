using Microsoft.AspNetCore.Mvc;

namespace Slottet.API.Controllers
{
    public class ResidentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
