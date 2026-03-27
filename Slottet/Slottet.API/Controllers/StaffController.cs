using Microsoft.AspNetCore.Mvc;

namespace Slottet.API.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
