using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Core.IRepository
{
    public interface IUnitOfWorkContext<T> : IUnitOfWork<T>, IDisposable where T : class
    {
        new T Context
        {
            get;
        }

        DbSet<TEntity> Set<TEntity>() where TEntity : EntityBase;

        void RegisterNew<TEntity>(TEntity entity) where TEntity : EntityBase;

        void RegisterNew<TEntity>(IEnumerable<TEntity> entities) where TEntity : EntityBase;

        void RegisterModified<TEntity>(TEntity entity, string[] properties = null) where TEntity : EntityBase;

        void RegisterModified<TEntity>(IEnumerable<TEntity> entities, string[] properties = null) where TEntity : EntityBase;

        void RegisterDeleted<TEntity>(TEntity entity) where TEntity : EntityBase;

        void RegisterDeleted<TEntity>(IEnumerable<TEntity> entities) where TEntity : EntityBase;

        void RegisterSoftDeleted<TEntity>(TEntity entity) where TEntity : EntityBase;
    }

}
