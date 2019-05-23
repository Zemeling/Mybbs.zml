using My.Common.Exceptions;
using My.Domain.Core.Extension;
using My.Domain.Core.IRepository;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Core.EFRepository
{
    public abstract class EFUnitOfWorkContextBase : IUnitOfWorkContext<DbContext>, IUnitOfWork<DbContext>, IDisposable
    {
        public abstract DbContext Context
        {
            get;
        }

        public bool IsCommitted
        {
            get;
            private set;
        }

        public int Commit()
        {
            if (this.IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = this.Context.SaveChanges();
                this.IsCommitted = true;
                return result;
            }
            catch (DbEntityValidationException validEx)
            {
                StringBuilder sberrors2 = new StringBuilder();
                foreach (DbEntityValidationResult entityValidationError in validEx.EntityValidationErrors)
                {
                    sberrors2.Append(string.Format("提交数据实体{0}检验错误：", entityValidationError.Entry.Entity));
                    foreach (DbValidationError validationError in entityValidationError.ValidationErrors)
                    {
                        sberrors2.Append(string.Format("数据校验错误Property：{0}，{1}。", validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
                throw PublicHelper.ThrowDataAccessException("提交数据验证时发生异常：" + sberrors2, validEx);
            }
            catch (DbUpdateConcurrencyException deConex)
            {
                DbEntityEntry entry = deConex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                throw PublicHelper.ThrowDataAccessException("提交数据并发时发生异常：" + deConex.Message, deConex);
            }
            catch (DbUpdateException dbupEx)
            {
                if (dbupEx.InnerException != null && dbupEx.InnerException.InnerException is SqlException)
                {
                    StringBuilder sberrors2 = new StringBuilder();
                    SqlException sqlEx = dbupEx.InnerException.InnerException as SqlException;
                    foreach (SqlError error in sqlEx.Errors)
                    {
                        string msg2 = (!string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlEx.Number))) ? DataHelper.GetSqlExceptionMessage(sqlEx.Number) : error.Message;
                        sberrors2.Append("SQL Error: " + error.Number + ", Message: " + msg2 + Environment.NewLine);
                    }
                    throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + sberrors2, sqlEx);
                }
                throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + dbupEx.Message, dbupEx);
            }
            catch (SqlException sqlex)
            {
                StringBuilder sberrors2 = new StringBuilder();
                foreach (SqlError error2 in sqlex.Errors)
                {
                    string msg2 = (!string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlex.Number))) ? DataHelper.GetSqlExceptionMessage(sqlex.Number) : error2.Message;
                    sberrors2.Append("SQL Error: " + error2.Number + ", Message: " + msg2 + Environment.NewLine);
                }
                throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + sberrors2.ToString(), sqlex);
            }
        }

        public void Rollback()
        {
            this.IsCommitted = false;
        }

        public void Dispose()
        {
            if (!this.IsCommitted)
            {
                this.Commit();
            }
            this.Context.Dispose();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : EntityBase
        {
            return this.Context.Set<TEntity>();
        }

        public void RegisterNew<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            EntityState state = this.Context.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                this.Context.Entry(entity).State = EntityState.Added;
            }
            this.IsCommitted = false;
        }

        public void RegisterNew<TEntity>(IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            try
            {
                this.Context.Configuration.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    this.RegisterNew(entity);
                }
            }
            finally
            {
                this.Context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public void RegisterModified<TEntity>(TEntity entity, string[] properties = null) where TEntity : EntityBase
        {
            this.Context.Detach(entity);
            if (this.Context.Entry(entity).State == EntityState.Detached)
            {
                this.Context.Set<TEntity>().Attach(entity);
            }
            if (properties != null)
            {
                this.Context.Entry(entity).State = EntityState.Unchanged;
                foreach (string property in properties)
                {
                    this.Context.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
            {
                this.Context.Entry(entity).State = EntityState.Modified;
            }
            if (((object)entity) is Entity)
            {
                this.Context.Entry(entity).Property("CreatedBy").IsModified = false;
                this.Context.Entry(entity).Property("CreatedDate").IsModified = false;
            }
            this.IsCommitted = false;
        }

        public void RegisterModified<TEntity>(IEnumerable<TEntity> entities, string[] properties = null) where TEntity : EntityBase
        {
            try
            {
                this.Context.Configuration.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    this.RegisterModified(entity, properties);
                }
            }
            finally
            {
                this.Context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public void RegisterDeleted<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            this.Context.Entry(entity).State = EntityState.Deleted;
            this.IsCommitted = false;
        }

        public void RegisterDeleted<TEntity>(IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            try
            {
                this.Context.Configuration.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    this.RegisterDeleted(entity);
                }
            }
            finally
            {
                this.Context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public void RegisterSoftDeleted<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            this.Context.Detach(entity);
            if (this.Context.Entry(entity).State == EntityState.Detached)
            {
                this.Context.Set<TEntity>().Attach(entity);
            }
            if (((object)entity) is Entity)
            {
                this.Context.Entry(entity).Property("IsDeleted").IsModified = true;
            }
            this.IsCommitted = false;
        }
    }
}
