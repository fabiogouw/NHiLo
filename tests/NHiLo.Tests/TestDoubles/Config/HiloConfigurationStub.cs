using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Common.Config;
using NHiLo.HiLo.Config;

namespace NHiLo.Tests.TestDoubles.Config
{
    public class HiloConfigurationStub : IHiLoConfiguration
    {
        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }

        public string ConnectionStringId { get; set; }

        public bool CreateHiLoStructureIfNotExists { get; set; }

        public int DefaultMaxLo { get; set; }

        public IEntityConfiguration GetEntityConfig(string entityName)
        {
            return GetEntityConfigFunction.Invoke(entityName);
        }

        public Func<string, IEntityConfiguration> GetEntityConfigFunction { get; set; }

        public string TableName { get; set; }

        public string NextHiColumnName { get; set; }

        public string EntityColumnName { get; set; }

        public HiLoStorageType StorageType { get; set; }

        public string ObjectPrefix { get; set; }
    }
}
