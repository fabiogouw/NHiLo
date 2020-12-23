using NHiLo.Common.Config;
using System.Configuration;

namespace NHiLo.HiLo.Config
{
    public class HiLoConfigurationBuilder
    {
        private readonly IConfigurationManager _configuration;
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
            return _configuration.GetKeyGeneratorConfigurationSection() ?? new HiLoConfigElement();
        }

        private ConnectionStringsSection GetConnectionStringsConfigurationSection()
        {
            var connectionStringsConfig = _configuration.GetConnectionStringsSection();
            CheckConsistenceOfConnectionStringsSection(connectionStringsConfig);
            return connectionStringsConfig;
        }

        private void CheckConsistenceOfConnectionStringsSection(ConnectionStringsSection connectionStringsConfig)
        {
            if (connectionStringsConfig == null || connectionStringsConfig.ConnectionStrings.Count == 0)
                throw new NHiLoException(ErrorCodes.NoConnectionStringAvailable);
        }

        private ConnectionStringSettings FindConnectionStringToBeUsedByHiLoConfiguration(IHiLoConfiguration hiloConfig)
        {
            ConnectionStringSettings connectionStringSettings;
            if (!string.IsNullOrEmpty(hiloConfig.ConnectionStringId))
            {
                connectionStringSettings = _connectionStringsConfig.ConnectionStrings[hiloConfig.ConnectionStringId];
                if (connectionStringSettings == null)
                    throw new NHiLoException(ErrorCodes.NoSpecifiedConnectionStringWasFound).WithInfo("ConnectionStringId", hiloConfig.ConnectionStringId);
            }
            else
                connectionStringSettings = _connectionStringsConfig.ConnectionStrings[_connectionStringsConfig.ConnectionStrings.Count - 1];   // or get the last connection string
            CheckConsistenceOfConnectionStringsSettings(connectionStringSettings);
            return connectionStringSettings;
        }

        private void CheckConsistenceOfConnectionStringsSettings(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
                throw new NHiLoException(ErrorCodes.NoConnectionStringAvailable);
        }

        private IHiLoConfiguration PrepareHiLoConfigurationWithConnectionString(IHiLoConfiguration hiloConfig, ConnectionStringSettings connectionStringSettings)
        {
            hiloConfig.ConnectionString = connectionStringSettings.ConnectionString;
            hiloConfig.ProviderName = connectionStringSettings.ProviderName;
            return hiloConfig;
        }
    }
}
