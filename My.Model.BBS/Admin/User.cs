using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.Admin
{
    [Table(Tables.UserInfo)]
    public class User : Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [DataMember]
        public Guid Userid { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        [DataMember]
        [MaxLength(30)]
        public string UserCode { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string NickName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public bool Sex { get; set; }
        /// <summary>
        /// 自我描述
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        public string MineDesc { get; set; }
        /// <summary>
        /// 自我介绍
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        public string MinePresent { get; set; }
        /// <summary>
        /// 所属行业ID
        /// </summary>
        [DataMember]
        public int TradeId { get; set; }
        /// <summary>
        /// 所属行业
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string TradeName { get; set; }
        /// <summary>
        /// 居住地
        /// </summary>
        [DataMember]
        [MaxLength(255)]
        public string Addr { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        [DataMember]
        [MaxLength(30)]
        public string RealName { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        [DataMember]
        [MaxLength(20)]
        public string SdCard { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        [DataMember]
        [MaxLength(20)]
        public string QQ { get; set; }
        /// <summary>
        /// 级别
        /// </summary>
        [DataMember]
        public int Elevel { get; set; }

    }
}
