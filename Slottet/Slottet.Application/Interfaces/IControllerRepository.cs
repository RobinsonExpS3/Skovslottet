using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Application.Interfaces {
    public interface IControllerRepository<T> {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T Entity);
        Task UpdateAsync(T Entity);
        Task DeleteAsync(Guid id);
    }
}
