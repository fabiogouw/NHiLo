using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NHiLo.HiLo.Config
{
    public class EntityConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EntityConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as EntityConfigElement).Name;
        }

        public EntityConfigElement this[int index]
        {
            get { return base.BaseGet(index) as EntityConfigElement; }
        }

        public virtual new EntityConfigElement this[string key]
        {
            get { return base.BaseGet(key) as EntityConfigElement; }
        }
    }
}
