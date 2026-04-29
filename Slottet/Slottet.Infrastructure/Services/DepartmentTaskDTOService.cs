using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class DepartmentTaskDTOService : IDepartmentTaskDTOService
    {
        private readonly SlottetDBContext _context;

        public DepartmentTaskDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentTaskDTO>> GetAll()
            {
            return await _context.DepartmentTasks
                .AsNoTracking()
                .OrderBy(dt => dt.DepartmentTaskName)
                .Select(dt => new DepartmentTaskDTO
                {
                    DepartmentID = dt.DepartmentID,
                    DepartmentTaskName = dt.DepartmentTaskName
                })
                .ToListAsync();
        }
    }
}
