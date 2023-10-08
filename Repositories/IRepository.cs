using System.Linq.Expressions;

namespace Market.Repositories;

public interface IRepository<TEntity, in TKey> where TEntity : class
{
   
    TEntity Create(TEntity entity);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
    TEntity Update(TKey id, TEntity entity);
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
}


