using My.Model.Core;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace My.Domain.Core.IRepository
{
    public interface IRepository<T> where T : EntityBase
    {
        bool Add(T entity, bool isSave = true);
        IList<T> Find(Expression<Func<T, bool>> predicate);
        IList<T> Find<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        IList<T> FindAll();
        IList<T> FindAll<K>(Expression<Func<T, K>> keySelector, bool ascending = true);
        bool Modify(T entity, bool isSave = true);
        bool Remove(T entity, bool isSave = true);
        int Remove(IEnumerable<T> entities, bool isSave = true);
        bool RemoveAll();
    }
}
