using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Linq.Expressions;

namespace Slottet.Infrastructure.Services
{
    public class DepartmentTaskDTOService : IDepartmentTaskDTOService
    {
        private readonly SlottetDBContext _context;

        public DepartmentTaskDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all department task objects and maps them to DTO objects.
        /// </summary>
        /// <returns>Returns a list of DepartmentTaskDTO objects.</returns>
        public async Task<IEnumerable<DepartmentTaskDTO>> GetAllDepartmentTasksAsync()
        {
            return await _context.DepartmentTasks
                .AsNoTracking()
                .OrderBy(dt => dt.DepartmentTaskName)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        /// <summary>
        /// Creates an expression that maps a DepartmentTask entity to a DepartmentTaskDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of DepartmentTask entities to DepartmentTaskDTO objects.</remarks>
        /// <returns>An expression that projects a DepartmentTask object into a DepartmentTaskDTO instance.</returns>
        private static Expression<Func<DepartmentTask, DepartmentTaskDTO>> MapToDtoExpression()
        {
            return departmentTask => new DepartmentTaskDTO
            {
                DepartmentID = departmentTask.DepartmentID,
                DepartmentTaskName = departmentTask.DepartmentTaskName
            };
        }
    }
}
