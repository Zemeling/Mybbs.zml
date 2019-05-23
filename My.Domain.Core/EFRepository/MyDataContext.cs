using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace My.Domain.Core.EFRepository
{
    public abstract class MyDataContext : DbContext
    {
        protected MyDataContext(string connectionString)
        : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            IEnumerable<DbEntityEntry> entries = base.ChangeTracker.Entries();
            foreach (DbEntityEntry item in entries)
            {
                if (item.Entity is Entity)
                {
                    Entity entity = (Entity)item.Entity;
                    if (item.State == EntityState.Added)
                    {
                        entity.CreatedBy = (string.IsNullOrWhiteSpace(entity.CreatedBy) ? Thread.CurrentPrincipal.Identity.Name : entity.CreatedBy);
                        entity.CreatedDate = ((entity.CreatedDate == DateTime.MinValue) ? DateTime.UtcNow : entity.CreatedDate);
                        entity.ModifiedBy = Thread.CurrentPrincipal.Identity.Name;
                        entity.ModifiedDate = DateTime.UtcNow;
                    }
                    if (item.State == EntityState.Modified)
                    {
                        entity.ModifiedBy = Thread.CurrentPrincipal.Identity.Name;
                        entity.ModifiedDate = DateTime.UtcNow;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}
