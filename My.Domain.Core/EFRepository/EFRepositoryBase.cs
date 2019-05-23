using My.Common.Exceptions;
using My.Common.Extension;
using My.Domain.Core.IRepository;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Core.EFRepository
{
    public abstract class EFRepositoryBase<T>: IDbRepository<T>, IRepository<T> where T : EntityBase
    {
        public IUnitOfWork<DbContext> UnitOfWork
        {
            get;
            private set;
        }

        protected IUnitOfWorkContext<DbContext> EFContext
        {
            get
            {
                if (this.UnitOfWork is IUnitOfWorkContext<DbContext>)
                {
                    return this.UnitOfWork as IUnitOfWorkContext<DbContext>;
                }
                throw new DataAccessException(string.Format("数据仓储上下文对象类型不正确，应为IRepositoryContext，实际为 {0}", this.UnitOfWork.GetType().Name));
            }
        }

        public virtual IQueryable<T> Entities
        {
            get
            {
                return this.EFContext.Set<T>();
            }
        }

        protected EFRepositoryBase(EFUnitOfWorkContextBase unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public virtual T FindById(params object[] keyValues)
        {
            return this.EFContext.Set<T>().Find(keyValues);
        }

        public virtual T FindByFilter(Expression<Func<T, bool>> predicate)
        {
            return Queryable.FirstOrDefault<T>((IQueryable<T>)this.EFContext.Set<T>().AsNoTracking(), predicate);
        }

        public virtual IList<T> FindAll()
        {
            return Enumerable.ToList<T>((IEnumerable<T>)this.EFContext.Set<T>().AsNoTracking());
        }

        public virtual IList<T> FindAll<K>(Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return ascending ? Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>((IQueryable<T>)Queryable.OrderBy<T, K>((IQueryable<T>)this.EFContext.Set<T>(), keySelector))) : Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>((IQueryable<T>)Queryable.OrderByDescending<T, K>((IQueryable<T>)this.EFContext.Set<T>(), keySelector)));
        }

        public virtual IList<T> Find(Expression<Func<T, bool>> predicate)
        {
            return Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate)));
        }

        public virtual IList<T> Find<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return ascending ? Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>((IQueryable<T>)Queryable.OrderBy<T, K>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate), keySelector))) : Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>((IQueryable<T>)Queryable.OrderByDescending<T, K>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate), keySelector)));
        }

        public virtual IList<T> FindPagenatedList<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return ascending ? Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>(Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderBy<T, K>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate), keySelector), (pageIndex - 1) * pageSize), pageSize))) : Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>(Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderByDescending<T, K>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate), keySelector), (pageIndex - 1) * pageSize), pageSize)));
        }

        public virtual IList<T> FindPagenatedList<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            IQueryable<T> q = QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate));
            return Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>(DynamicQueryable.SingleOrderBy<T>(q, sortProperty, ascending), (pageIndex - 1) * pageSize), pageSize));
        }

        public virtual Tuple<int, IList<T>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            IQueryable<T> q = QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate));
            int recordCount = Queryable.Count<T>(q);
            List<T> data = Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>(DynamicQueryable.SingleOrderBy<T>(q, sortProperty, ascending), (pageIndex - 1) * pageSize), pageSize));
            return new Tuple<int, IList<T>>(recordCount, (IList<T>)data);
        }

        public virtual Tuple<int, IList<T>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            IQueryable<T> databaseItems = QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate));
            int recordCount = Queryable.Count<T>(databaseItems);
            return ascending ? new Tuple<int, IList<T>>(recordCount, (IList<T>)Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderBy<T, K>(databaseItems, keySelector), (pageIndex - 1) * pageSize), pageSize))) : new Tuple<int, IList<T>>(recordCount, (IList<T>)Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderByDescending<T, K>(databaseItems, keySelector), (pageIndex - 1) * pageSize), pageSize)));
        }

        public virtual IList<T> FindPagenatedTreeList<K>(int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return ascending ? Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>(Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderBy<T, K>(Queryable.Where<T>(QueryableExtensions.Include<T, IEnumerable<IEnumerable<IList<T>>>>((IQueryable<T>)this.EFContext.Set<T>(), including), predicate), keySelector), (pageIndex - 1) * pageSize), pageSize))) : Enumerable.ToList<T>((IEnumerable<T>)QueryableExtensions.AsNoTracking<T>(Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderByDescending<T, K>(Queryable.Where<T>(QueryableExtensions.Include<T, IEnumerable<IEnumerable<IList<T>>>>((IQueryable<T>)this.EFContext.Set<T>(), including), predicate), keySelector), (pageIndex - 1) * pageSize), pageSize)));
        }

        public virtual IList<T> FindPagenatedTreeList<K>(int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            IQueryable<T> q = QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>(QueryableExtensions.Include<T, IEnumerable<IEnumerable<IList<T>>>>((IQueryable<T>)this.EFContext.Set<T>(), including), predicate));
            return Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>(DynamicQueryable.SingleOrderBy<T>(q, sortProperty, ascending), (pageIndex - 1) * pageSize), pageSize));
        }

        public virtual Tuple<int, IList<T>> FindPagenatedTreeListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            IQueryable<T> databaseItems = QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>(QueryableExtensions.Include<T, IEnumerable<IEnumerable<IList<T>>>>((IQueryable<T>)this.EFContext.Set<T>(), including), predicate));
            int recordCount = Queryable.Count<T>(databaseItems);
            return ascending ? new Tuple<int, IList<T>>(recordCount, (IList<T>)Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderBy<T, K>(databaseItems, keySelector), (pageIndex - 1) * pageSize), pageSize))) : new Tuple<int, IList<T>>(recordCount, (IList<T>)Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>((IQueryable<T>)Queryable.OrderByDescending<T, K>(databaseItems, keySelector), (pageIndex - 1) * pageSize), pageSize)));
        }

        public virtual Tuple<int, IList<T>> FindPagenatedTreeListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            IQueryable<T> q = QueryableExtensions.AsNoTracking<T>(Queryable.Where<T>(QueryableExtensions.Include<T, IEnumerable<IEnumerable<IList<T>>>>((IQueryable<T>)this.EFContext.Set<T>(), including), predicate));
            int recordCount = Queryable.Count<T>(q);
            List<T> data = Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>(DynamicQueryable.SingleOrderBy<T>(q, sortProperty, ascending), (pageIndex - 1) * pageSize), pageSize));
            return new Tuple<int, IList<T>>(recordCount, (IList<T>)data);
        }

        public virtual bool Add(T entity, bool isSave = true)
        {
            this.EFContext.RegisterNew(entity);
            return isSave && this.EFContext.Commit() > 0;
        }

        public virtual int Add(IEnumerable<T> entities, bool isSave = true)
        {
            this.EFContext.RegisterNew(entities);
            return isSave ? this.EFContext.Commit() : 0;
        }

        public virtual bool Remove(object id, bool isSave = true)
        {
            T entity = this.EFContext.Set<T>().Find(id);
            return entity != null && this.Remove(entity, isSave);
        }

        public virtual bool Remove(T entity, bool isSave = true)
        {
            this.EFContext.RegisterDeleted(entity);
            return isSave && this.EFContext.Commit() > 0;
        }

        public virtual int Remove(IEnumerable<T> entities, bool isSave = true)
        {
            this.EFContext.RegisterDeleted(entities);
            return isSave ? this.EFContext.Commit() : 0;
        }

        public virtual int Remove(Expression<Func<T, bool>> predicate, bool isSave = true)
        {
            List<T> entities = Enumerable.ToList<T>((IEnumerable<T>)Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate));
            return (entities.Count > 0) ? this.Remove(entities, isSave) : 0;
        }

        public virtual bool RemoveAll()
        {
            List<T> entities = Enumerable.ToList<T>((IEnumerable<T>)this.EFContext.Set<T>());
            if (entities.Count > 0)
            {
                this.Remove(entities, false);
                return this.EFContext.Commit() > 0;
            }
            return true;
        }

        public virtual bool SoftRemove(object id, bool isSave = true)
        {
            T entity = this.EFContext.Set<T>().Find(id);
            return entity != null && this.SoftRemove(entity, isSave);
        }

        public virtual bool SoftRemove(T entity, bool isSave = true)
        {
            if (!(((object)entity) is Entity))
            {
                throw new ArgumentException("T must be an Entity type that is derived from the Entity class in the namespace Com.Domain.Core.");
            }
            Entity aEntity = ((object)entity) as Entity;
            aEntity.IsDeleted = true;
            this.EFContext.RegisterSoftDeleted(entity);
            return isSave && this.EFContext.Commit() > 0;
        }

        public virtual int SoftRemove(IEnumerable<T> entities, bool isSave = true)
        {
            foreach (T entity in entities)
            {
                if (!(((object)entity) is Entity))
                {
                    throw new ArgumentException("T must be an Entity type that is derived from the Entity class in the namespace Com.Domain.Core.");
                }
                Entity aEntity = ((object)entity) as Entity;
                aEntity.IsDeleted = true;
                this.EFContext.RegisterSoftDeleted(entity);
            }
            return isSave ? this.EFContext.Commit() : 0;
        }

        public virtual int SoftRemove(Expression<Func<T, bool>> predicate, bool isSave = true)
        {
            List<T> entities = Enumerable.ToList<T>((IEnumerable<T>)Queryable.Where<T>((IQueryable<T>)this.EFContext.Set<T>(), predicate));
            return (entities.Count > 0) ? this.SoftRemove(entities, isSave) : 0;
        }

        public virtual bool Modify(T entity, bool isSave = true)
        {
            this.EFContext.RegisterModified(entity, null);
            return isSave && this.EFContext.Commit() > 0;
        }

        public virtual bool Modify(T entity, string[] properties, bool isSave = true)
        {
            this.EFContext.RegisterModified(entity, properties);
            return isSave && this.EFContext.Commit() > 0;
        }

        public virtual int Modify(IEnumerable<T> entities, string[] properties = null, bool isSave = true)
        {
            this.EFContext.RegisterModified(entities, properties);
            return isSave ? this.EFContext.Commit() : 0;
        }

        public virtual int GetRecordCount()
        {
            return Queryable.Count<T>((IQueryable<T>)this.EFContext.Set<T>().AsNoTracking());
        }

        public List<T> SqlQuery<T>(string query)
        {
            return Enumerable.ToList<T>((IEnumerable<T>)this.EFContext.Context.Database.SqlQuery<T>(query, new object[0]));
        }

        public bool ExecuteSql(string query, params object[] parameters)
        {
            return this.EFContext.Context.Database.ExecuteSqlCommand(query, parameters) > 0;
        }
    }
}
