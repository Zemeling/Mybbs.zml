using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    public sealed class ParseException : Exception
    {
        private int position;

        public int Position
        {
            get
            {
                return this.position;
            }
        }

        public ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }

        public override string ToString()
        {
            return string.Format("{0} (at index {1})", this.Message, this.position);
        }
    }
}
