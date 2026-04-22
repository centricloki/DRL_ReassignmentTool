using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DRL.Library;

namespace DRL.Model.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(long id);
        //T GetByUniqueId(Guid uniqueId);
        ActionStatus Insert(T entity);
        ActionStatus Update(T entity);
        ActionStatus Delete(T entity);
        IQueryable<T> GetAll();

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        ActionStatus RemoveRange(Expression<Func<T, bool>> predicate);
        void SetModified<K>(K entity) where K : class;

        ActionStatus UpdateRange(List<T> entities);
        IQueryable<T> GetByWhere(Expression<Func<T, bool>> predicate);
    }
}
