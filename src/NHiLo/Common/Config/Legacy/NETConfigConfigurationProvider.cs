using Microsoft.Extensions.Configuration;
using NHiLo.Common.Legacy;
using System.Configuration;

namespace NHiLo.Common.Config.Legacy
{
    public class NETConfigConfigurationProvider : ConfigurationProvider, IConfigurationSource
    {
        public override void Load()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                Data.Add($"ConnectionStrings:{connectionString.Name}:ConnectionString", connectionString.ConnectionString);
                Data.Add($"ConnectionStrings:{connectionString.Name}:ProviderName", connectionString.ProviderName);
            }

            var config = ConfigurationManager.GetSection("nhilo") as KeyGeneratorConfig;
            Data.Add("NHiLo:ConnectionStringId", config.HiloKeyGenerator.ConnectionStringId);
            Data.Add("NHiLo:ProviderName", config.HiloKeyGenerator.ProviderName);
            Data.Add("NHiLo:CreateHiLoStructureIfNotExists", config.HiloKeyGenerator.CreateHiLoStructureIfNotExists.ToString());
            Data.Add("NHiLo:DefaultMaxLo", config.HiloKeyGenerator.DefaultMaxLo.ToString());
            Data.Add("NHiLo:TableName", config.HiloKeyGenerator.TableName);

            Data.Add("NHiLo:NextHiColumnName", config.HiloKeyGenerator.NextHiColumnName);
            Data.Add("NHiLo:EntityColumnName", config.HiloKeyGenerator.EntityColumnName);
            Data.Add("NHiLo:StorageType", config.HiloKeyGenerator.StorageType.GetDescription());
            Data.Add("NHiLo:ObjectPrefix", config.HiloKeyGenerator.ObjectPrefix);

            var entitiesConfigCollection = config.HiloKeyGenerator.Entities;
            for (int i = 0; i < entitiesConfigCollection.Count; i++)
            {
                Data.Add($"NHiLo:Entities:{ entitiesConfigCollection[i].Name }:MaxLo", entitiesConfigCollection[i].MaxLo.ToString());
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return this;
        }
    }
}
