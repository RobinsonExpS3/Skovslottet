using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.Application.Services
{
    public class MedicineService : IMedicineService
    {
        public async Task<MedicineAPI_DTO> CreateAsync(MedicineAPI_DTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MedicineAPI_DTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<MedicineAPI_DTO> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Guid id, MedicineAPI_DTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
