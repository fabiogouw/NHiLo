using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Represents the configuration for each entity (aka. table) and its max lo value.
    /// </summary>
    public class EntityConfigElement : ConfigurationElement, IEntityConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public virtual bool Name
        {
            get { return (bool)this["name"]; }
        }

        [ConfigurationProperty("maxLo", IsRequired = false, DefaultValue = 100)]
        public virtual int MaxLo
        {
            get { return (int)this["mxLo"]; }
        }
    }
}
