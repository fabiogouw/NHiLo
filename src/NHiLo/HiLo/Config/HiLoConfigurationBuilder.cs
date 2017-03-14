using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NHiLo.Common;
using NHiLo.Common.Config;

namespace NHiLo.HiLo.Config
{
    public class HiLoConfigurationBuilder
    {
        private IConfigurationManager _configuration;
        private ConnectionStringsSection _connectionStringsConfig;

        public HiLoConfigurationBuilder(IConfigurationManager configuration)
        {
            _configuration = configuration;
        }

        public IHiLoConfiguration Build()
        {
            var hiloConfig = GetHiLoConfigurationSection();
            _connectionStringsConfig = GetConnectionStringsConfigurationSection();
            var connectionStringSettings = FindConnectionStringToBeUsedByHiLoConfiguration(hiloConfig);
            hiloConfig = PrepareHiLoConfigurationWithConnectionString(hiloConfig, connectionStringSettings);
            return hiloConfig;
        }

        private IHiLoConfiguration GetHiLoConfigurationSection()
        {
            var keyGeneratorConfig = _configuration.GetSection<IKeyGeneratorConfiguration>("nhilo");
            var hiloConfig = keyGeneratorConfig != null ? keyGeneratorConfig.HiloKeyGenerator : new HiLoConfigElement();
            return hiloConfig;
        }

        private ConnectionStringsSection GetConnectionStringsConfigurationSection()
        {
            var connectionStringsConfig = _configuration.GetSection<ConnectionStringsSection>("connectionStrings");
            CheckConsistenceOfConnectionStringsSection(connectionStringsConfig);
            return connectionStringsConfig;
        }

        private void CheckConsistenceOfConnectionStringsSection(ConnectionStringsSection connectionStringsConfig)
        {
            if (connectionStringsConfig == null || connectionStringsConfig.ConnectionStrings.Count == 0)
                throw new NHiloException(ErrorCodes.NoConnectionStringAvailable);
        }

        private ConnectionStringSettings FindConnectionStringToBeUsedByHiLoConfiguration(IHiLoConfiguration hiloConfig)
        {
            ConnectionStringSettings connectionStringSettings = null;
            if (!string.IsNullOrEmpty(hiloConfig.ConnectionStringId))
                connectionStringSettings = _connectionStringsConfig.ConnectionStrings[hiloConfig.ConnectionStringId];
            else
                connectionStringSettings = _connectionStringsConfig.ConnectionStrings[_connectionStringsConfig.ConnectionStrings.Count - 1];   // or get the last connection string
            CheckConsistenceOfConnectionStringsSettings(connectionStringSettings);
            return connectionStringSettings;
        }

        private void CheckConsistenceOfConnectionStringsSettings(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
                throw new NHiloException(ErrorCodes.NoConnectionStringAvailable);
        }

        private IHiLoConfiguration PrepareHiLoConfigurationWithConnectionString(IHiLoConfiguration hiloConfig, ConnectionStringSettings connectionStringSettings)
        {
            hiloConfig.ConnectionString = connectionStringSettings.ConnectionString;
            hiloConfig.ProviderName = connectionStringSettings.ProviderName;
            return hiloConfig;
        }
    }
}
