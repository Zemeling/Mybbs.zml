using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace My.Model.BBS.New
{
    [Table(Tables.NewKeep)]
    public class NewKeeps : EntityBase
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
        public Guid KeepUserId { get; set; }
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
        public virtual News New { get; set; }
    }
}
