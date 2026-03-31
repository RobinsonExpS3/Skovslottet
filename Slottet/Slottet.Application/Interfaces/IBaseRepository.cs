using System.Collections.ObjectModel;

namespace Slottet.Application.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task InitializeAsync();
        ObservableCollection<T> Items { get; }
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);

    }
}
