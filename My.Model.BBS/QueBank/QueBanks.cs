using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.QueBank
{
    [Table(Tables.QueBank)]
    public class QueBanks : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 题库名称
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string QueName { get; set; }
        /// <summary>
        /// 题库描述
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string QueRemark { get; set; }

        public virtual IList<Ques> Ques { get; set; }
    }
}
