using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Post
{
    [Table(Tables.PostKeep)]
    public class PostKeeps: EntityBase
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 收藏人
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string KeepUser { get; set; }
        /// <summary>
        /// 收藏人ID
        /// </summary>
        [DataMember]
        public string KeepUserId { get; set; }
        /// <summary>
        /// 收藏时间
        /// </summary>
        [DataMember]
        public DateTime KeepTime { get; set; }
        /// <summary>
        /// 是否取消收藏
        /// </summary>
        public bool IsOutKeep { get; set; }
        [ForeignKey("Pid")]
        public virtual Posts Posts { get; set; }
    }
}
