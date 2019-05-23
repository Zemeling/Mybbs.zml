using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace My.Model.Core.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class TreeNode<T> : Entity where T : EntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(128)]
        public string Name { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [MaxLength(128)]
        public string TreeCode { get; set; }
        /// <summary>
        /// 分支
        /// </summary>
        public bool Leaf { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        [ForeignKey("ParentId")]
        public virtual List<T> ChildNodes { get; set; }

        public virtual T ParentNode { get; set; }

        public TreeNode() { this.ChildNodes = new List<T>(); }
    }
}
