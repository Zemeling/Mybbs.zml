using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Model.BBS.Constants
{
    public sealed class Tables
    {
        private const string Prx = "bbs_";

        /// <summary>
        /// 用户表
        /// </summary>
        public const string UserInfo = Prx + "Users";
        /// <summary>
        /// 行业表
        /// </summary>
        public const string IndustryInfo = Prx + "Idustries";
        /// <summary>
        /// 类型表
        /// </summary>
        public const string GenreInfo = Prx + "Genres";

        /// <summary>
        /// 资讯表
        /// </summary>
        public const string NewsInfo = Prx + "News";
        /// <summary>
        /// 资讯图片表
        /// </summary>
        public const string NewImage = Prx + "NewImgs";
        /// <summary>
        /// 资讯视频表
        /// </summary>
        public const string NewVideo = Prx + "NewVideoes";
        /// <summary>
        /// 资讯评论表
        /// </summary>
        public const string NewDiscuss = Prx + "NewDiscusses";
        /// <summary>
        /// 资讯收藏表
        /// </summary>
        public const string NewKeep = Prx + "NewKeeps";

        /// <summary>
        /// 帖子
        /// </summary>
        public const string Post = Prx + "Posts";
        /// <summary>
        /// 帖子回复
        /// </summary>
        public const string PostReply = Prx + "PostReplies";
        /// <summary>
        /// 帖子收藏
        /// </summary>
        public const string PostKeep = Prx + "PostKeeps";

        /// <summary>
        /// 挑战
        /// </summary>
        public const string Dare = Prx + "Dares";
        /// <summary>
        /// 挑战明细
        /// </summary>
        public const string DareDatail = Prx + "DareDatails";
        /// <summary>
        /// 挑战题目
        /// </summary>
        public const string DareQue = Prx + "DareQues";

        /// <summary>
        /// 题库
        /// </summary>
        public const string QueBank = Prx + "QueBanks";
        /// <summary>
        /// 题目
        /// </summary>
        public const string Ques= Prx + "Ques";
        /// <summary>
        /// 题目选项
        /// </summary>
        public const string QueOption = Prx + "QueOptions";
    }
}
