using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NHiLo.HiLo.Config.Legacy
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
