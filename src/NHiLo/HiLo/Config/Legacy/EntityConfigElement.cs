using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NHiLo.HiLo.Config.Legacy
{
    /// <summary>
    /// Represents the configuration for each entity (aka. table) and its max lo value.
    /// </summary>
    public class EntityConfigElement : ConfigurationElement, IEntityConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public virtual string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("maxLo", IsRequired = false, DefaultValue = 100)]
        public virtual int MaxLo
        {
            get { return (int)this["maxLo"]; }
        }
    }
}
