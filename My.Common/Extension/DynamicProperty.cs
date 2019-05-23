﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    public class DynamicProperty
    {
        private string name;

        private Type type;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
        }

        public DynamicProperty(string name, Type type)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (type == (Type)null)
            {
                throw new ArgumentNullException("type");
            }
            this.name = name;
            this.type = type;
        }
    }
}
