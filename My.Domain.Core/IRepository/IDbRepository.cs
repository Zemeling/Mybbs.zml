using My.Model.Core;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Core.IRepository
{
    public interface IDbRepository<T> : IRepository<T> where T : EntityBase
    {
        IQueryable<T> Entities { get; }

        int Add(IEnumerable<T> entities, bool isSave = true);
        T FindByFilter(Expression<Func<T, bool>> predicate);
        T FindById(params object[] keyValues);
        IList<T> FindPagenatedList<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        IList<T> FindPagenatedList<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true);
        Tuple<int, IList<T>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        bool Modify(T entity, string[] properties, bool isSave = true);
        int Modify(IEnumerable<T> entities, string[] properties = null, bool isSave = true);
        bool Remove(object id, bool isSave = true);
        bool SoftRemove(object id, bool isSave = true);
        bool SoftRemove(T entity, bool isSave = true);
    }
}
