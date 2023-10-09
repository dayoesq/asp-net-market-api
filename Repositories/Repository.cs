using System.Linq.Expressions;
using System.Runtime.Serialization;
using Market.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Market.Repositories;
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;

    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        return entity!;

    }

    public TEntity Create(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        return entity;
    }

    public TEntity Update(TKey id, TEntity entity)
    {
        var existingEntity = _context.Set<TEntity>().Find(id);

        if (existingEntity == null)
        {
            throw new EntityNotFoundException($"Entity with id {id} not found.");
        }

        // Update scalar properties
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);

        if (_context.ChangeTracker.HasChanges())
        {
            _context.SaveChanges();
        }

        return existingEntity;
    }



    public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await GetAsync(predicate);
        if (entity == null)
            return false;

        _context.Set<TEntity>().Remove(entity);
        return true;
    }


}

[Serializable]
internal class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}