using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IPaymentMethodDTOService
    {
        Task<IEnumerable<ResidentLookupDTO>> GetAllAsync();
    }
}
