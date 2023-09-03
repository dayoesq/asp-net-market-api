using System.Linq.Expressions;
using Market.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Market.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet;
    }
    
    public async Task<T?> GetById(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task Add(T entity)
    {
        await Task.Run(() => _dbSet.Add(entity));
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task Delete(T entity)
    {
        await Task.Run(() => _dbSet.Remove(entity));
    }
}