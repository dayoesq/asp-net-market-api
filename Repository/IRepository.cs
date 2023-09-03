using System.Linq.Expressions;

namespace Market.Repository;

using System;
using System.Linq;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T?> GetById(Expression<Func<T, bool>> predicate);
    Task Add(T entity);
    void Update(T entity);
    Task Delete(T entity);
}




