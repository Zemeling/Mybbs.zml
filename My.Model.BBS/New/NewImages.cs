using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.New
{
    [Table(Tables.NewImage)]
    public class NewImages : EntityCheckBase
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
        /// 图片名称
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        public string PicName { get; set; }
        /// <summary>
        /// 图片来源
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string PicFrom { get; set; }
        /// <summary>
        /// 图片描述
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string PicRemark { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string PicPath { get; set; }
        
        [ForeignKey("Pid")]
        public virtual News News { get; set; }
    }
}
