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
    public class EntityConfigElement : IEntityConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public virtual string Name
        {
            get; internal set;
        }

        [ConfigurationProperty("maxLo", IsRequired = false, DefaultValue = 100)]
        public virtual int MaxLo
        {
            get; internal set;
        }
    }
}
