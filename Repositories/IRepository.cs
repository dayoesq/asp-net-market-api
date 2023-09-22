using System.Linq.Expressions;

namespace Market.Repositories;

public interface IRepository<TEntity, in TKey> where TEntity : class
{
   
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> GetAsync(TKey id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TKey id, TEntity entity);
    Task<bool> DeleteAsync(TKey id);
}


