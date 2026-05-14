using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces {
    public interface IResidentDTOService {
        Task<IEnumerable<EditResidentDTO>> GetAllResidentsAsync();
        Task<EditResidentDTO?> GetResidentByIdAsync(Guid id);
        Task<EditResidentDTO?> PostResidentAsync(EditResidentDTO dto);
        Task<bool> PutResidentAsync(Guid id, EditResidentDTO dto);
        Task<bool> DeleteResidentAsync(Guid id);
        Task<bool> DeactivateResidentAsync(Guid id, CancellationToken ct = default);

        Task<List<ResidentLookupDTO>> GetResidentGroceryDaysAsync();
        Task<List<ResidentLookupDTO>> GetResidentPaymentMethodsAsync();
        Task<List<ResidentCardDto>> GetResidentCardsAsync(CancellationToken ct = default);
        Task<bool> SwapResidentSortOrderAsync(Guid residentIdA, Guid residentIdB, CancellationToken ct = default);
        Task<bool> UpdateMedicineTimesAsync(Guid residentId, List<TimeOnly> times, CancellationToken ct = default);
        Task<bool> UpdatePaymentMethodsAsync(Guid residentId, List<Guid> paymentMethodIds, CancellationToken ct = default);
        Task<bool> UpdateGroceryDayAsync(Guid residentId, Guid groceryDayId, CancellationToken ct = default);
    }
}
