using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Post
{
    [Table(Tables.Post)]
    public class Posts : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 帖子内容
        /// </summary>
        [DataMember]
        public string PostContent { get; set; }
        /// <summary>
        /// 帖子标题
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string PostTitle { get; set; }
        /// <summary>
        /// 帖子类型名称
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        public string PostTypeName { get; set; }
        /// <summary>
        /// 帖子类型ID
        /// </summary>
        [DataMember]
        public int PostTypeId { get; set; }
        /// <summary>
        /// 发帖时间
        /// </summary>
        [DataMember]
        public DateTime PostTime { get; set; }
        /// <summary>
        /// 发帖人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string PostUser { get; set; }
        /// <summary>
        /// 发帖人ID
        /// </summary>
        [DataMember]
        public Guid PostUserId { get; set; }
        /// <summary>
        /// 是否精华帖
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        public bool IsMarrow { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        public bool IsTop { get; set; }
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

        public virtual IList<PostReplies> PostReplies { get; set; }
        public virtual IList<PostKeeps> PostKeeps { get; set; }
    }
}
