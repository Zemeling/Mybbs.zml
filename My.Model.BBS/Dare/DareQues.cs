using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Dare
{
    [Table(Tables.DareQue)]
    public class DareQues : EntityBase
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 题目ID
        /// </summary>
        [DataMember]
        public int QueId { get; set; }
        [ForeignKey("Pid")]
        public virtual Dares Dares { get; set; }
    }
}
