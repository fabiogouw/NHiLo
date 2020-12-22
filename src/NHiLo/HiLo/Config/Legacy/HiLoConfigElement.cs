using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NHiLo.HiLo.Config.Legacy
{
    public class HiLoConfigElement : ConfigurationElement, IHiLoConfiguration
    {
        [ConfigurationProperty("connectionStringId", IsRequired = false, DefaultValue = "")]
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
        public EntityConfigElementCollection Entities
        {
            get { return this["entities"] as EntityConfigElementCollection; }
        }

        [ConfigurationProperty("tableName", IsRequired = false, DefaultValue = "NHILO")]
        public string TableName
        {
            get { return (string)this["tableName"]; }
        }

        [ConfigurationProperty("nextHiColumnName", IsRequired = false, DefaultValue = "NEXT_HI")]
        public string NextHiColumnName
        {
            get { return (string)this["nextHiColumnName"]; }
        }

        [ConfigurationProperty("entityColumnName", IsRequired = false, DefaultValue = "ENTITY")]
        public string EntityColumnName
        {
            get { return (string)this["entityColumnName"]; }
        }

        [ConfigurationProperty("storageType", IsRequired = false, DefaultValue = "Table")]
        public Common.Config.HiLoStorageType StorageType
        {
            get { return (Common.Config.HiLoStorageType)this["storageType"]; }
        }

        [ConfigurationProperty("objectPrefix", IsRequired = false, DefaultValue = "")]
        public string ObjectPrefix
        {
            get { return (string)this["objectPrefix"]; }
        }
    }
}
