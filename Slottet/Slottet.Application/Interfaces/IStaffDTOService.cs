using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IStaffDTOService
    {
        Task<IEnumerable<EditStaffDTO>> GetAllStaffAsync();
        Task<EditStaffDTO?> GetStaffByIdAsync(Guid id);
        Task<EditStaffDTO?> PostStaffAsync(EditStaffDTO dto);
        Task<bool> PutStaffAsync(Guid id, EditStaffDTO dto);
        Task<bool> DeleteStaffAsync(Guid id);
    }
}
