namespace AibolitAPI.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int page, int size);
    Task<IEnumerable<T>> GetAllAsync(int page, int size, Func<IQueryable<T>, IQueryable<T>>? include);
    Task<T> GetByIdAsync(Guid id);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task SoftDeleteAsync(Guid id);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}