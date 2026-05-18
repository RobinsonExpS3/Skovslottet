using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IStaffDTOService
    {
        Task<IEnumerable<EditStaffDto>> GetAllStaffAsync();
        Task<EditStaffDto?> GetStaffByIdAsync(Guid id);
        Task<EditStaffDto?> PostStaffAsync(EditStaffDto dto);
        Task<bool> PutStaffAsync(Guid id, EditStaffDto dto);
        Task<bool> DeleteStaffAsync(Guid id);
    }
}
