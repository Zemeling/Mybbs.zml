using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Core.IRepository
{
    public interface IUnitOfWork<T> where T : class
    {
        T Context
        {
            get;
        }

        bool IsCommitted
        {
            get;
        }

        int Commit();

        void Rollback();
    }
}
