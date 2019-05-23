using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Post
{
    [Table(Tables.PostReply)]
    public class PostReplies : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [DataMember]
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string ReplyUser { get; set; }
        /// <summary>
        /// 回复人ID
        /// </summary>
        [DataMember]
        public Guid ReplyUserId { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [DataMember]
        public DateTime ReplyTime { get; set; }
        /// <summary>
        /// 违反规则ID
        /// </summary>
        [DataMember]
        public int? OutRuleId { get; set; }
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
        [ForeignKey("Pid")]
        public virtual Posts Posts { get; set; }
    }
}
