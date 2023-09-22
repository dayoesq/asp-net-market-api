using System.Linq.Expressions;

namespace Market.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
   
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(int id, TEntity entity);
    Task<bool> DeleteAsync(int id);
}


