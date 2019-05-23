using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.New
{
    [Table(Tables.NewVideo)]
    public class NewVideoes: EntityCheckBase
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Pid { get; set; }
        /// <summary>
        /// 视频名称
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string VioName { get; set; }
        /// <summary>
        /// 视频来源
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string VidFrom { get; set; }
        /// <summary>
        /// 视频描述
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string VioRemork { get; set; }
        /// <summary>
        /// 视频地址
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string VioPath { get; set; }
        
        [ForeignKey("Pid")]
        public virtual News New { get; set; }
    }
}
