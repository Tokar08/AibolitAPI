using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

//NOTE :Тестовый базовый репозиторий
//TODO: Заменить потом для каждой реализации репозитория, ибо будут дополняться методы для большинства сущностей!
public class Repository<T> : IRepository<T> where T : class
{
    private readonly AibolitDbContext _context;
    private readonly DbSet<T> _dbSet;

    protected Repository(AibolitDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(int page, int size)
    {
        var collection = _dbSet
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        return await collection;
    }

    public async Task<IEnumerable<T>> GetAllAsync(int page, int size, Func<IQueryable<T>, IQueryable<T>> include)
    {
        var query = include(_dbSet);
        return await query.Skip((page - 1) * size).Take(size).ToListAsync();
    }


    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id)
               ?? throw new InvalidOperationException("Entity not found.");
    }


    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        var entityWithActiveFlag = entity as dynamic;
        entityWithActiveFlag.IsActive = false;
        await UpdateAsync(entity);
    }
}