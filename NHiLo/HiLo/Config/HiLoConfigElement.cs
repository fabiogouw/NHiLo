using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Common;
using System.Configuration;

namespace NHiLo.HiLo.Config
{
    public class HiLoConfigElement : KeyGeneratorConfigElement, IHiLoConfiguration
    {
        [ConfigurationProperty("connectionStringId", IsRequired = false, DefaultValue="")]
        public string ConnectionStringId
        {
            get { return (string)this["connectionStringId"]; }
        }

        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }

        [ConfigurationProperty("createHiLoStructureIfNotExists", IsRequired = false, DefaultValue = true)]
        public bool CreateHiLoStructureIfNotExists
        {
            get { return (bool)this["createHiLoStructureIfNotExists"]; }
        }

        [ConfigurationProperty("defaultMaxLo", IsRequired = false, DefaultValue = 100)]
        public int DefaultMaxLo
        {
            get { return (int)this["defaultMaxLo"]; }
        }

        public IEntityConfiguration GetEntityConfig(string entityName)
        {
            return Entities[entityName];
        }

        [ConfigurationProperty("entities", IsRequired = false)]
        private EntityConfigElementCollection Entities
        {
            get { return this["entities"] as EntityConfigElementCollection; }
        }
    }
}
