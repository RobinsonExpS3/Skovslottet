using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentTaskController : Controller
    {
        private readonly IDepartmentTaskDTOService _departmentTaskService;

        public DepartmentTaskController(IDepartmentTaskDTOService departmentTaskService)
        {
            _departmentTaskService = departmentTaskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentTaskDTO>>> GetAllAsync()
        {
            var departmentTasks = await _departmentTaskService.GetAll();
            return Ok(departmentTasks);
        }
    }
}
