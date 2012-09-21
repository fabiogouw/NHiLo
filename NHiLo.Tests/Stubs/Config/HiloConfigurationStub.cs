using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.HiLo.Config;

namespace NHiLo.Tests.Stubs.Config
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
    }
}
