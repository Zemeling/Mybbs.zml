using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.BaseData
{
    [Table(Tables.IndustryInfo)]
    public class IndustryInfo : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 行业名称
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string TradeName { get; set; }
        /// <summary>
        /// 行业编号
        /// </summary>
        [DataMember]
        [MaxLength(30)]
        public string TradeCode { get; set; }
        /// <summary>
        /// 级次
        /// </summary>
        [DataMember]
        public int Elevel { get; set; }
    }
}
