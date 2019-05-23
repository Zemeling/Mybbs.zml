using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace My.Model.Core.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    [ProtoContract]
    public abstract class EntityCheckBase : EntityBase
    {
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
    }
}
