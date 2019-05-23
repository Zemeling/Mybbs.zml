using My.Common.Util;
using My.Domain.Core.EFRepository;
using My.Model.BBS.Admin;
using My.Model.BBS.BaseData;
using My.Model.BBS.Dare;
using My.Model.BBS.New;
using My.Model.BBS.Post;
using My.Model.BBS.QueBank;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.DataAccess.BBS
{
    public class MyBBSContext: MyDataContext
    {
        private static readonly string _connectionString = System.Configuration.ConfigurationManager.AppSettings["MyDataContext"];

        public MyBBSContext()
           : base(_connectionString)
        {
            Database.SetInitializer<MyBBSContext>(null);
        }

        #region 用户管理

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<User> Users { get; set; }
        #endregion
        #region 基础数据
        /// <summary>
        /// 类型表
        /// </summary>
        public DbSet<GenreInfo> GenreInfos { get; set; }
        /// <summary>
        /// 行业表
        /// </summary>
        public DbSet<IndustryInfo> IndustryInfos { get; set; }
        #endregion
        #region 资讯管理
        /// <summary>
        /// 资讯表
        /// </summary>
        public DbSet<News> News { get; set; }
        /// <summary>
        /// 资讯图片表
        /// </summary>
        public DbSet<NewImages> NewImages { get; set; }
        /// <summary>
        /// 资讯视频表
        /// </summary>
        public DbSet<NewVideoes> NewVideoes { get; set; }
        /// <summary>
        /// 资讯评论表
        /// </summary>
        public DbSet<NewDiscusses> NewDiscusses { get; set; }
        /// <summary>
        /// 资讯收藏表
        /// </summary>
        public DbSet<NewKeeps> NewKeeps { get; set; }
        #endregion
        #region 挑战管理
        /// <summary>
        /// 挑战表
        /// </summary>
        public DbSet<Dares> Dares { get; set; }
        /// <summary>
        /// 挑战明细
        /// </summary>
        public DbSet<DareDetail> DareDetails { get; set; }
        /// <summary>
        /// 挑战问题
        /// </summary>
        public DbSet<DareQues> DareQues { get; set; }
        #endregion
        #region 帖子管理
        /// <summary>
        /// 帖子
        /// </summary>
        public DbSet<Posts> Posts { get; set; }
        /// <summary>
        /// 帖子回复
        /// </summary>
        public DbSet<PostReplies> PostReplies { get; set; }
        /// <summary>
        /// 帖子收藏
        /// </summary>
        public DbSet<PostKeeps> PostKeeps { get; set; }
        #endregion
        #region 题库
        /// <summary>
        /// 题库
        /// </summary>
        public DbSet<QueBanks> QueBanks { get; set; }
        /// <summary>
        /// 题目
        /// </summary>
        public DbSet<Ques> Ques { get; set; }
        /// <summary>
        /// 题目选项
        /// </summary>
        public DbSet<QueOptions> QueOptions { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }
            base.OnModelCreating(modelBuilder);


        }

    }
}
