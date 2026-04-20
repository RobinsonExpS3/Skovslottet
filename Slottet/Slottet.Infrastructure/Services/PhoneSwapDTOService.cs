using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class PhoneSwapDTOService : IPhoneSwapDTOService
    {

        private readonly SlottetDBContext _context;

        public PhoneSwapDTOService(SlottetDBContext context)
        {
            _context = context;
        }
        public Task<PhoneSwapDTO> CreateAsync(PhoneSwapDTO dto)
        {
            var phoneSwap = new PhoneSwap
            {
                PhoneID = Guid.NewGuid(),
                PhoneNumber = dto.PhoneNumber,
                StaffName = dto.StaffName
            };
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PhoneSwapDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PhoneSwapDTO?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid id, PhoneSwapDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
