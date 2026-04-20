using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.Application.Services
{
    public class MedicineService : IMedicineService
    {
        public Task<MedicineAPI_DTO> CreateAsync(MedicineAPI_DTO dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MedicineAPI_DTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MedicineAPI_DTO> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid id, MedicineAPI_DTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
