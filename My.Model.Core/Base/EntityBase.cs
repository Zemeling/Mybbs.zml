using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace My.Model.Core.Base
{
    [DataContract(IsReference = true)]
    [ProtoContract]
    public abstract class EntityBase
    {
        
    }
}
