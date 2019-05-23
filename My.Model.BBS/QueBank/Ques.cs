using My.Enums;
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

namespace My.Model.BBS.QueBank
{
    [Table(Tables.Ques)]
    public class Ques : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 题目内容
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string QueContent { get; set; }
        /// <summary>
        /// 题目类型
        /// </summary>
        public TopicType TopicType { get; set; }
        /// <summary>
        /// 题目答案
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string QueAmswer { get; set; }
        /// <summary>
        /// 题目来源
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string QueFrom { get; set; }
        /// <summary>
        /// 题目等级
        /// </summary>
        [DataMember]
        public int QueLevel { get; set; }

        [ForeignKey("Pid")]
        public virtual QueBanks QueBanks { get; set; }

        public virtual IList<QueOptions> QueOptions { get; set; }
    }
}
