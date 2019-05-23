using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    internal class Signature : IEquatable<Signature>
    {
        public DynamicProperty[] properties;

        public int hashCode;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            this.properties = properties.ToArray();
            this.hashCode = 0;
            foreach (DynamicProperty property in properties)
            {
                this.hashCode ^= (property.Name.GetHashCode() ^ property.Type.GetHashCode());
            }
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is Signature && this.Equals((Signature)obj);
        }

        public bool Equals(Signature other)
        {
            if (this.properties.Length != other.properties.Length)
            {
                return false;
            }
            for (int i = 0; i < this.properties.Length; i++)
            {
                if (this.properties[i].Name != other.properties[i].Name || this.properties[i].Type != other.properties[i].Type)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
