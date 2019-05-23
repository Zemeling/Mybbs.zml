using My.Model.BBS.Constants;
using My.Model.Core.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace My.Model.BBS.BaseData
{
    [Table(Tables.GenreInfo)]
    public class GenreInfo : TreeNode<GenreInfo>
    {
        /// <summary>
        /// 是否系统默认
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        public bool IsSystem { get; set; }
        /// <summary>
        /// 是否是资讯
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        public bool IsNews { get; set; }

    }
}
