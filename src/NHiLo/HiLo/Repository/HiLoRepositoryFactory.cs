using NHilo.HiLo.Repository;
using NHiLo.HiLo.Config;
using System;
using System.Collections.Concurrent;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// Factory that creates repositories based on the provider specified in the connection string configuration.
    /// </summary>
    public class HiLoRepositoryFactory : IHiLoRepositoryFactory
    {
        private delegate IHiLoRepository CreateIHiLoRepositoryFunction(string entityName, IHiLoConfiguration config);

        /// <summary>
        /// Relates each kind of provider to a function that actually creates the correct repository. If a new provider is add, this constant should change.
        /// </summary>
        private readonly ConcurrentDictionary<string, CreateIHiLoRepositoryFunction> _factoryFunctions;

        public HiLoRepositoryFactory()
        {
            _factoryFunctions = new ConcurrentDictionary<string, CreateIHiLoRepositoryFunction>()
            {
                ["Microsoft.Data.SqlClient"] = (entityName, config) => GetSqlServerRepository(entityName, config),
                ["MySql.Data.MySqlClient"] = (entityName, config) => new MySqlHiLoRepository(entityName, config),
                ["System.Data.OracleClient"] = (entityName, config) => new OracleHiLoRepository(entityName, config),
                ["NHilo.InMemory"] = (entityName, config) => new InMemoryHiloRepository(entityName, config)
            };
        }

        public IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config)
        {
            string provider = config.ProviderName;
            if (string.IsNullOrWhiteSpace(provider))
                throw new NHiloException(ErrorCodes.NoProviderName);
            if (!_factoryFunctions.ContainsKey(provider))
                throw new NHiloException(ErrorCodes.ProviderNotImplemented).WithInfo("Provider Name", provider);
            var repository = new ExceptionWrapperRepository(() => _factoryFunctions[provider](entityName, config));
            repository.PrepareRepository();
            return repository;
        }

        private IHiLoRepository GetSqlServerRepository(string entityName, IHiLoConfiguration config)
        {
            if (config.StorageType == Common.Config.HiLoStorageType.Sequence)
            {
                return new SqlServerSequenceHiLoRepository(entityName, config);
            }
            return new SqlServerHiLoRepository(entityName, config);
        }

        public void RegisterRepository(string providerName, Func<IHiLoRepository> funcCreateRepository)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException($"Provider name cannot be registered with an empty value.");
            if (!_factoryFunctions.ContainsKey(providerName))
                _factoryFunctions.TryAdd(providerName, (entityName, config) => funcCreateRepository());
        }
    }
}
