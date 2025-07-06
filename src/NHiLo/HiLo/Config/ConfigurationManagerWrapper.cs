using Microsoft.Extensions.Configuration;
using NHiLo.Common.Config;
using System.Configuration;
using System.Linq;

namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Converts the underlying config model from the .NET framework to the NHilo's config model.
    /// </summary>
    public class ConfigurationManagerWrapper : IConfigurationManagerWrapper
    {
        /*
         * {
         *   "ConnectionStrings": {
         *       "NHiLo": {
         *          "ConnectionString": "Data Source=|DataDirectory|\\Database1.sdf;Persist Security Info=False;",
         *              "ProviderName": "System.Data.SqlServerCe.4.0"
         *       }
         *   },
         *   "NHiLo": {
         *      "Providers": [
         *          {"Name": "Microsoft.Data.Sqlite", "Type": "NHiLo.HiLo.Repository.SQLiteHiLoRepositoryProvider, NHilLo.Repository.SQLite"}
         *      ]
         *      "ConnectionStringId": "",
         *      "ProviderName": "",
         *      "CreateHiLoStructureIfNotExists": true,
         *      "DefaultMaxLo": 100,
         *      "TableName": "",
         *      "NextHiColumnName": "",
         *      "EntityColumnName": "",
         *      "StorageType": "",
         *      "ObjectPrefix": "",
         *      "Entities": {
         *          "myEntity": { "maxLo" : 10 }
         *      }
         *   }
         * }
         */

        private readonly IConfiguration _configuration;

        public ConfigurationManagerWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IHiLoConfiguration GetKeyGeneratorConfigurationSection()
        {
            var configuration = new HiLoConfigElement
            {
                ConnectionStringId = _configuration.GetValue<string>("NHiLo:ConnectionStringId", string.Empty),
                ProviderName = _configuration.GetValue<string>("NHiLo:ProviderName", string.Empty),
                CreateHiLoStructureIfNotExists = _configuration.GetValue("NHiLo:CreateHiLoStructureIfNotExists", true),
                DefaultMaxLo = _configuration.GetValue("NHiLo:DefaultMaxLo", 100),
                TableName = _configuration.GetValue("NHiLo:TableName", "NHILO"),
                NextHiColumnName = _configuration.GetValue("NHiLo:NextHiColumnName", "NEXT_HI"),
                EntityColumnName = _configuration.GetValue("NHiLo:EntityColumnName", "ENTITY"),
                StorageType = _configuration.GetValue("NHiLo:StorageType", HiLoStorageType.Table),
                ObjectPrefix = _configuration.GetValue("NHiLo:ObjectPrefix", string.Empty),
                EntityNameValidationTimeout = _configuration.GetValue<int?>("NHiLo:EntityNameValidationTimeout", null),
                Entities = _configuration.GetSection("NHiLo:Entities").GetChildren().Select(v =>
                    (IEntityConfiguration)new EntityConfigElement()
                    {
                        Name = v.GetValue<string>("Name"),
                        MaxLo = v.GetValue("MaxLo", 10)
                    }).ToList(),
                Providers = _configuration.GetSection("NHiLo:Providers").GetChildren().Select(v =>
                    (IRepositoryProviderElement)new RepositoryProviderElement()
                    {
                        Type = v.GetValue<string>("Type")
                    }).ToList()
            };
            return configuration;
        }

        public ConnectionStringsSection GetConnectionStringsSection()
        {
            var connectionStringsSection = new ConnectionStringsSection();
            var connectionStrings = _configuration.GetSection("ConnectionStrings");
            foreach (var connectionString in connectionStrings.GetChildren())
            {
                var children = connectionString.GetChildren();
                var connectionStringSetting = new ConnectionStringSettings()
                {
                    Name = connectionString.Key,
                    ConnectionString = children.SingleOrDefault(x => x.Key.ToLower() == "connectionstring")?.Value ?? connectionString.Value,
                    ProviderName = children.SingleOrDefault(x => x.Key.ToLower() == "providername")?.Value ?? _configuration.GetValue<string>("NHiLo:ProviderName", string.Empty)
                };
                connectionStringsSection.ConnectionStrings.Add(connectionStringSetting);
            }
            return connectionStringsSection;
        }
    }
}
