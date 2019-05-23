using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.New
{
    [Table(Tables.NewsInfo)]
    public class News : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 资讯名称
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string NewsName { get; set; }
        /// <summary>
        /// 资讯标题
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        public string NewsTitle { get; set; }
        /// <summary>
        /// 资讯列表图片
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string NewsListImg { get; set; }
        /// <summary>
        /// 资讯内容
        /// </summary>
        [DataMember]
        public string NewsContent { get; set; }
        /// <summary>
        /// 资讯来源
        /// </summary>
        [DataMember]
        [MaxLength(30)]
        public string NewsFrom { get; set; }
        /// <summary>
        /// 资讯来源URL
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string NewsFromUrl { get; set; }
        /// <summary>
        /// 资讯类型
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        public string NewsTypeName { get; set; }
        /// <summary>
        /// 资讯类型ID
        /// </summary>
        [DataMember]
        public int NewsTypeId { get; set; }
        
        /// <summary>
        /// 违反规则ID
        /// </summary>
        public int? OutRoleId { get; set; }
        /// <summary>
        ///检查人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string CheckUser { get; set; }
        /// <summary>
        /// 检查人ID.归属系统/系统检查时ID为空
        /// </summary>
        [DataMember]
        public Guid? CheckUserId { get; set; }
        /// <summary>
        /// 检查时间。系统检查时不记录时间
        /// </summary>
        [DataMember]
        public DateTime? CheckTime { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [DataMember]
        public bool IsPopular { get; set; }
        ///<summary>
        /// 发布时间
        /// </summary>
        [DataMember]
        public DateTime IssueTime { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string IssueUser { get; set; }
        /// <summary>
        /// 发布人ID.归属系统/系统发布时ID为空
        /// </summary>
        [DataMember]
        public Guid? IssueUserId { get; set; }

        public virtual IList<NewImages> NewImages { get; set; }
        public virtual IList<NewVideoes> NewVideoes { get; set; }
        public virtual IList<NewDiscusses> NewDiscusses { get; set; }
        public virtual IList<NewKeeps> NewKeeps { get; set; }
    }
}
