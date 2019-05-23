using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    internal class DynamicOrdering
    {
        public Expression Selector;

        public bool Ascending;
    }
}
