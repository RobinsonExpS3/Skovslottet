using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using Slottet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Slottet.Infrastructure.Services
{
    public class SwapPhoneDTOService : ISwapPhoneDTOService
    {
        private readonly SlottetDBContext _context;

        public SwapPhoneDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SwapPhoneDTO>> GetAllAsync()
        {
            return await _context.Phones
                .AsNoTracking()
                .OrderBy(p => p.PhoneID)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        public Task<SwapPhoneDTO?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid id, SwapPhoneDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
