using NHiLo.HiLo.Repository;
using NHiLo.HiLo.Config;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// Factory that creates repositories based on the provider specified in the connection string configuration.
    /// </summary>
    public class HiLoRepositoryFactory : IHiLoRepositoryFactory
    {
        private delegate IHiLoRepository CreateIHiLoRepositoryFunction(IHiLoConfiguration config);

        private static readonly object _lock = new object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Relates each kind of provider to a function that actually creates the correct repository. If a new provider is add, this constant should change.
        /// </summary>
        private static readonly ConcurrentDictionary<string, CreateIHiLoRepositoryFunction> _factoryFunctions = 
            new ConcurrentDictionary<string, CreateIHiLoRepositoryFunction>()
            {
                /*["Microsoft.Data.SqlClient"] = (config) => GetSqlServerRepository(config),
                ["MySqlConnector"] = (config) => new MySqlHiLoRepository(config),
                ["MySql.Data.MySqlClient"] = (config) => new MySqlHiLoRepository(config),   //compatibility
                ["System.Data.OracleClient"] = (config) => new OracleHiLoRepository(config),
                ["Microsoft.Data.Sqlite"] = (config) => new SqliteHiLoRepository(config),*/
                ["NHiLo.InMemory"] = (config) => new InMemoryHiloRepository()
            };

        public HiLoRepositoryFactory(IHiLoConfiguration config)
        {
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    var providers = config.Providers.Select(p => new { p.Name, Type = Type.GetType(p.Type) })
                        .Where(p => typeof(IHiLoRepositoryProvider).IsAssignableFrom(p.Type));
                    foreach (var provider in providers)
                    {
                        bool hasParameterlessCtor = provider.Type.GetConstructor(Type.EmptyTypes) == null;
                        if (hasParameterlessCtor)
                            throw new InvalidOperationException($"Type {provider.Type.FullName} must have a parameterless constructor.");
                        RegisterRepository(provider.Name, (IHiLoRepositoryProvider)Activator.CreateInstance(provider.Type));
                    }
                    _isInitialized = true;
                }
            }
        }

        public IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config)
        {
            string provider = config.ProviderName;
            if (string.IsNullOrWhiteSpace(provider))
                throw new NHiLoException(ErrorCodes.NoProviderName);
            if (!_factoryFunctions.ContainsKey(provider))
                throw new NHiLoException(ErrorCodes.ProviderNotImplemented).WithInfo("Provider Name", provider);
            var repository = new ExceptionWrapperRepository(() => _factoryFunctions[provider](config));
            repository.PrepareRepository(entityName);
            return repository;
        }

        /// <summary>
        /// Register a new repository to be used to store hi values.
        /// </summary>
        /// <param name="providerName">The name of the custom respository provider.</param>
        /// <param name="funcCreateRepository">A function that creates new instances of the repository.</param>
        private static void RegisterRepository(string providerName, IHiLoRepositoryProvider provider)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException($"Provider name cannot be registered with an empty value.");
            lock (_factoryFunctions)
            {
                if (!_factoryFunctions.ContainsKey(providerName))
                    _factoryFunctions.TryAdd(providerName, (config) => provider.Build(config));
            }
        }
    }
}
