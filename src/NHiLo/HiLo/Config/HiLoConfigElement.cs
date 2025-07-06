using NHiLo.Common.Config;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NHiLo.HiLo.Config
{
    public class HiLoConfigElement : IHiLoConfiguration
    {
        public HiLoConfigElement()
        {
            Entities = new EntityConfigElement[] { }.ToList<IEntityConfiguration>();
        }

        public List<IRepositoryProviderElement> Providers { get; set; }

        public string ConnectionStringId
        {
            get; internal set;
        }

        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }

        public bool CreateHiLoStructureIfNotExists
        {
            get; internal set;
        }

        public int DefaultMaxLo
        {
            get; internal set;
        }

        public IEntityConfiguration GetEntityConfig(string entityName)
        {
            var entityConfiguration = Entities.SingleOrDefault(v => v.Name == entityName);
            return entityConfiguration ?? new EntityConfigElement()
            {
                Name = entityName,
                MaxLo = DefaultMaxLo
            };
        }

        internal List<IEntityConfiguration> Entities
        {
            private get; set;
        }

        public string TableName
        {
            get; internal set;
        }

        public string NextHiColumnName
        {
            get; internal set;
        }

        public string EntityColumnName
        {
            get; internal set;
        }

        public HiLoStorageType StorageType
        {
            get; internal set;
        }

        public string ObjectPrefix
        {
            get; internal set;
        }

        public int? EntityNameValidationTimeout
        {
            get; internal set;
        }
    }
}