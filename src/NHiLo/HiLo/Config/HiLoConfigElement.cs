using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Common;
using System.Configuration;
using NHiLo.Common.Config;

namespace NHiLo.HiLo.Config
{
    public class HiLoConfigElement : KeyGeneratorConfigElement, IHiLoConfiguration
    {
        public HiLoConfigElement()
        {
            // TODO: inicializar aqui corretamente
            //Entities = new List<EntityConfigElement>();
        }

        [ConfigurationProperty("connectionStringId", IsRequired = false, DefaultValue="")]
        public string ConnectionStringId
        {
            get; internal set;
        }

        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }

        [ConfigurationProperty("createHiLoStructureIfNotExists", IsRequired = false, DefaultValue = true)]
        public bool CreateHiLoStructureIfNotExists
        {
            get; internal set;
        }

        [ConfigurationProperty("defaultMaxLo", IsRequired = false, DefaultValue = 100)]
        public int DefaultMaxLo
        {
            get; internal set;
        }

        public IEntityConfiguration GetEntityConfig(string entityName)
        {
            return Entities.SingleOrDefault(v => v.Name == entityName);
        }

        [ConfigurationProperty("entities", IsRequired = false)]
        public List<IEntityConfiguration> Entities
        {
            get; internal set;
        }

        [ConfigurationProperty("tableName", IsRequired = false, DefaultValue = "NHILO")]
        public string TableName
        {
            get; internal set;
        }

        [ConfigurationProperty("nextHiColumnName", IsRequired = false, DefaultValue = "NEXT_HI")]
        public string NextHiColumnName
        {
            get; internal set;
        }

        [ConfigurationProperty("entityColumnName", IsRequired = false, DefaultValue = "ENTITY")]
        public string EntityColumnName
        {
            get; internal set;
        }

        [ConfigurationProperty("storageType", IsRequired = false, DefaultValue = "Table")]
        public HiLoStorageType StorageType
        {
            get; internal set;
        }

        [ConfigurationProperty("objectPrefix", IsRequired = false, DefaultValue = "")]
        public string ObjectPrefix
        {
            get; internal set;
        }
    }
}
