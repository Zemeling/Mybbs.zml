using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Dare
{
    [Table(Tables.Dare)]
    public class Dares : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 挑战类型
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        public string DareType { get; set; }
        /// <summary>
        /// 挑战时长
        /// </summary>
        [DataMember]
        public int DareLast { get; set; }
        /// <summary>
        /// 挑战标题
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string DareTitle { get; set; }
        /// <summary>
        /// 挑战描述
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string DareRemark { get; set; }
        /// <summary>
        /// 挑战提示
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string DareRemad { get; set; }
        /// <summary>
        /// 挑战题数
        /// </summary>
        [DataMember]
        public int DareQusNumber { get; set; }
        /// <summary>
        /// 挑战题目来自的题库ID
        /// </summary>
        [DataMember]
        public int QusBankId { get; set; }
        /// <summary>
        /// 挑战题目来自的题库
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string QusBankName { get; set; }
        /// <summary>
        /// 挑战题库生成规则
        /// </summary>
        [DataMember]
        public int? DareQueRuleId { get; set; }
        /// <summary>
        /// 挑战分数
        /// </summary>
        [DataMember]
        public int DareTotalScore { get; set; }
        /// <summary>
        /// 平均正确率
        /// </summary>
        public int AvgRate { get; set; }
        /// <summary>
        /// 挑战认可数
        /// </summary>
        public int DareAppNum { get; set; }

        public virtual IList<DareDetail> DareDetails { get; set; }
        public virtual IList<DareQues> DareQues { get; set; }
    }
}
