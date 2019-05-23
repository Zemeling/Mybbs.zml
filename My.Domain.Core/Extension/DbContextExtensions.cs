using My.Model.Core.Base;
using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace My.Domain.Core.Extension
{
    public static class DbContextExtensions
    {
        public static void Detach<T>(this DbContext context, T entity) where T : EntityBase
        {
            try
            {
                ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<T> objSet = objContext.CreateObjectSet<T>();
                EntityKey entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);
                object foundEntity = default(object);
                if (objContext.TryGetObjectByKey(entityKey, out foundEntity))
                {
                    objContext.Detach(foundEntity);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void AdapterSaveChanges(this DbContext context)
        {
            try
            {
                ((IObjectContextAdapter)context).ObjectContext.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
    }

}
