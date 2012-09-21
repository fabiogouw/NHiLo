using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Common;
using NHiLo.HiLo.Config;
using System.Configuration;
using NHiLo.HiLo.Repository;
using NHiLo.Common.Config;

namespace NHiLo.HiLo
{
    /// <summary>
    /// Factory that creates <see cref="IKeyGeneratorFactory<long>"/> for client usage.
    /// </summary>
    public class HiLoGeneratorFactory : IKeyGeneratorFactory<long>
    {
        private readonly static object _lock = new object();
        // When instantiated, key generators are stored in a static field. That's how NHilo keeps the id generation globally per AppDomain.
        private readonly static Dictionary<string, IKeyGenerator<long>> _keyGenerators = new Dictionary<string, IKeyGenerator<long>>();
        private readonly IHiLoRepositoryFactory _repositoryFactory;
        private readonly IHiLoConfiguration _config;

        public HiLoGeneratorFactory() : 
            this(null, null)
        {
            // This is the default constructor for client use. It'll pass null values so the protector constructor can provide the default implementation
        }

        internal HiLoGeneratorFactory(IHiLoRepositoryFactory repositoryFactory, IHiLoConfiguration config)
        {
            _config = config ?? new HiLoConfigurationBuilder(new ConfigurationManagerWrapper()).Build();
            _repositoryFactory = repositoryFactory ?? new HiLoRepositoryFactory();
        }

        public IKeyGenerator<long> GetKeyGenerator(string entityName)
        {
            lock (_lock)
            {
                if (!_keyGenerators.ContainsKey(entityName))
                    _keyGenerators.Add(entityName, CreateKeyGenerator(entityName));
                return _keyGenerators[entityName];
            }
        }

        private IKeyGenerator<long> CreateKeyGenerator(string entityName)
        {
            var entityConfig = _config.GetEntityConfig(entityName);
            return new HiLoGenerator(_repositoryFactory.GetRepository(entityName, _config), entityConfig != null ? entityConfig.MaxLo : _config.DefaultMaxLo);
        }
    }
}
