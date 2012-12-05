using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.HiLo.Config;

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
        private Dictionary<string, CreateIHiLoRepositoryFunction> _factoryFunctions = new Dictionary<string, CreateIHiLoRepositoryFunction>()
        {
            { "System.Data.SqlClient", (entityName, config) => new SqlServerHiLoRepository(entityName, config) },
            { "MySql.Data.MySqlClient", (entityName, config) => new MySqlHiLoRepository(entityName, config) },
            { "System.Data.SqlServerCe.3.5", (entityName, config) => new SqlServerCeHiLoRepository(entityName, config) },
            { "System.Data.SqlServerCe.4.0", (entityName, config) => new SqlServerCeHiLoRepository(entityName, config) },
            { "System.Data.OracleClient", (entityName, config) => new OracleHiLoRepository(entityName, config) }
        };

        public IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config)
        {
            IHiLoRepository repository = null;
            string provider = config.ProviderName;
            if (!_factoryFunctions.ContainsKey(provider))
                throw new ArgumentException("Repository not implemented (yet).", provider);
            repository = _factoryFunctions[provider](entityName, config);
            repository.PrepareRepository();
            return repository;
        }
    }
}
