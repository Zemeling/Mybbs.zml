using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Dare
{
    [Table(Tables.DareDatail)]
    public class DareDetail : EntityBase
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 挑战得分
        /// </summary>
        [DataMember]
        public int Score { get; set; }
        /// <summary>
        /// 挑战人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string DareUser { get; set; }
        /// <summary>
        /// 挑战人ID
        /// </summary>
        [DataMember]
        public Guid DareUserId { get; set; }
        /// <summary>
        /// 挑战时间
        /// </summary>
        [DataMember]
        public DateTime DareTime { get; set; }
        /// <summary>
        /// 已挑战时长
        /// </summary>
        [DataMember]
        public int HasDareLong { get; set; }
        /// <summary>
        /// 剩余挑战时间
        /// </summary>
        [DataMember]
        public int LastLong { get; set; }
        /// <summary>
        /// 正确率
        /// </summary>
        [DataMember]
        public int Correcrate { get; set; }
        /// <summary>
        /// 是否结束
        /// </summary>
        [DataMember]
        public bool IsEnd { get; set; }
        /// <summary>
        /// 是否放弃
        /// </summary>
        [DataMember]
        public bool IsQuit { get; set; }
        [ForeignKey("Pid")]
        public virtual Dares Dares { get; set; }
    }
}
