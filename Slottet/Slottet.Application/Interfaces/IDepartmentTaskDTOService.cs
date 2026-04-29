using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IDepartmentTaskDTOService
    {
        Task<IEnumerable<DepartmentTaskDTO>> GetAll();
    }
}
