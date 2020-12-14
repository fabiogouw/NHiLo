using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using NHiLo.Common.Config;
using NHiLo.Common;
using System.Configuration;

namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Converts the underlying config model from the .NET framework to the nHilo's config model.
    /// </summary>
    public class ConfigurationManagerWrapper : IConfigurationManager
    {
        /*
         * {
         *   "ConnectionStrings": {
         *       "NHiLo": {
         *          "ConnectionString": "Data Source=|DataDirectory|\\Database1.sdf;Persist Security Info=False;",
         *              "ProviderName": "System.Data.SqlServerCe.4.0"
         *       }
         *   },
         *   "NHilo": {
         *      "ConnectionStringId": "",
         *      "CreateHiLoStructureIfNotExists": true,
         *      "DefaultMaxLo": 100,
         *      "TableName": "",
         *      "NextHiColumnName": "",
         *      "EntityColumnName": "",
         *      "StorageType": "",
         *      "ObjectPrefix": "",
         *      "Entities": [
         *          { "name": "", "maxLo" : 10 }   
         *      ]
         *   }
         * }
         */

        private IConfiguration _configuration;

        public ConfigurationManagerWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IHiLoConfiguration GetKeyGeneratorConfigurationSection()
        {
            var configuration = new HiLoConfigElement();
            configuration.ConnectionStringId = _configuration.GetValue<string>("NHilo:ConnectionStringId");
            configuration.CreateHiLoStructureIfNotExists = _configuration.GetValue("NHilo:CreateHiLoStructureIfNotExists", true);
            configuration.DefaultMaxLo = _configuration.GetValue("NHilo:DefaultMaxLo", 100);
            configuration.TableName = _configuration.GetValue("NHilo:TableName", "NHILO");
            configuration.NextHiColumnName = _configuration.GetValue("NHilo:NextHiColumnName", "NEXT_HI");
            configuration.EntityColumnName = _configuration.GetValue("NHilo:EntityColumnName", "ENTITY");
            configuration.StorageType = _configuration.GetValue("NHilo:StorageType", HiLoStorageType.Table);
            configuration.ObjectPrefix = _configuration.GetValue("NHilo:ObjectPrefix", string.Empty);
            configuration.Entities = _configuration.GetSection("NHilo:ObjectPrefix").GetChildren().Select(v =>
                (IEntityConfiguration) new EntityConfigElement()
                {
                    Name = v.GetValue<string>("Name"),
                    MaxLo = v.GetValue("MaxLo", 10)
                }
            ).ToList();
            return configuration;
        }

        public ConnectionStringsSection GetConnectionStringsSection()
        {
            var connectionStringsSection = new ConnectionStringsSection();
            var connectionStrings = _configuration.GetSection("ConnectionStrings");
            foreach(var connectionString in connectionStrings.GetChildren())
            {
                var children = connectionString.GetChildren();
                var connectionStringSetting = new ConnectionStringSettings()
                {
                    Name = connectionString.Key,
                    ConnectionString = children.SingleOrDefault(x => x.Key.ToLower() == "connectionstring")?.Value ?? connectionString.Value,
                    ProviderName = children.SingleOrDefault(x => x.Key.ToLower() == "providername")?.Value ?? ""
                };
                connectionStringsSection.ConnectionStrings.Add(connectionStringSetting);
            }
            return connectionStringsSection;
        }
    }
}
