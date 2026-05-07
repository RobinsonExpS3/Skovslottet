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

        /// <summary>
        /// Gets all department tasks as DTO objects.
        /// </summary>
        /// <returns>Returns all department tasks.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentTaskDTO>>> GetAllDepartmentTasksAsync()
        {
            var departmentTasks = await _departmentTaskService.GetAllDepartmentTasksAsync();
            return Ok(departmentTasks);
        }
    }
}
