using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.QueBank
{
    [Table(Tables.QueOption)]
    public class QueOptions : EntityBase
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 选项内容
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string OptionCon { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int RankOrder { get; set; }

        [ForeignKey("Pid")]
        public virtual Ques Ques { get; set; }
    }
}
