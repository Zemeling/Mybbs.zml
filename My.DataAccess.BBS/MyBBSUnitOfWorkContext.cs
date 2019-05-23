using My.Domain.Core.EFRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.DataAccess.BBS
{
    public class MyBBSUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public MyBBSUnitOfWorkContext()
        {
            MyBbsContext = new MyBBSContext();
            MyBbsContext.Configuration.LazyLoadingEnabled = false;
            MyBbsContext.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        ///     获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public override DbContext Context
        {
            get { return MyBbsContext; }
        }

        /// <summary>
        ///     获取或设置 默认数据访问上下文对象
        /// </summary>
        //[Import(typeof(DbContext))]
        protected MyBBSContext MyBbsContext { get; private set; }
    }
}
