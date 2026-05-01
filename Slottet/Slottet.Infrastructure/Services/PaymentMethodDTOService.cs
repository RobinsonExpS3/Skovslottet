using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Linq.Expressions;

namespace Slottet.Infrastructure.Services
{
    public class PaymentMethodDTOService : IPaymentMethodDTOService
    {
        private readonly SlottetDBContext _context;

        public PaymentMethodDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all payment method objects and maps them to lookup DTO objects.
        /// </summary>
        /// <returns>Returns a list of ResidentLookupDTO objects.</returns>
        public async Task<IEnumerable<ResidentLookupDTO>> GetAllPaymentMethodsAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .OrderBy(m => m.PaymentMethodName)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        /// <summary>
        /// Creates an expression that maps a PaymentMethod entity to a ResidentLookupDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of PaymentMethod entities to ResidentLookupDTO objects.</remarks>
        /// <returns>An expression that projects a PaymentMethod object into a ResidentLookupDTO instance.</returns>
        private static Expression<Func<PaymentMethod, ResidentLookupDTO>> MapToDtoExpression()
        {
            return paymentMethod => new ResidentLookupDTO
            {
                ID = paymentMethod.PaymentMethodID,
                Name = paymentMethod.PaymentMethodName
            };
        }
    }
}
