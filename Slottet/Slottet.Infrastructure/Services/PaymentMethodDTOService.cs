using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class PaymentMethodDTOService : IPaymentMethodDTOService
    {
        private readonly SlottetDBContext _context;

        public PaymentMethodDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResidentLookupDTO>> GetAllAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .OrderBy(m => m.PaymentMethodName)
                .Select(m => new ResidentLookupDTO
                {
                    ID = m.PaymentMethodID,
                    Name = m.PaymentMethodName
                })
                .ToListAsync();
        }
    }
}
