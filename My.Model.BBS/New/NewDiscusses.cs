using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.New
{
    [Table(Tables.NewDiscuss)]
    public class NewDiscusses : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string DisContent { get; set; }
        /// <summary>
        /// 评论人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string DisUser { get; set; }
        /// <summary>
        /// 评论人ID
        /// </summary>
        [DataMember]
        public Guid DisUserId { get; set; }
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
        public virtual News New { get; set; }
    }
}
