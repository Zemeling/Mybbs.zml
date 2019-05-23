using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace My.Enums
{
    [DataContract]
    public enum TopicType
    {
        [EnumMember]
        [Description("单选")]
        SingleCheck = 0,
        [EnumMember]
        [Description("多选")]
        MoreCheck = 1
    }
}
