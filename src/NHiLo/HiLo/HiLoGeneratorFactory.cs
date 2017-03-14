using NHiLo.Common;
using NHiLo.Common.Config;
using NHiLo.HiLo;
using NHiLo.HiLo.Config;
using NHiLo.HiLo.Repository;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NHiLo // this should be available at the root namespace
{
    /// <summary>
    /// Factory that creates <see cref="IKeyGeneratorFactory"/> for client usage.
    /// </summary>
    public class HiLoGeneratorFactory : IKeyGeneratorFactory<long>
    {
        private readonly static object _lock = new object();
        // When instantiated, key generators are stored in a static field. That's how NHilo keeps the id generation globally per AppDomain.
        private readonly static Dictionary<string, IKeyGenerator<long>> _keyGenerators = new Dictionary<string, IKeyGenerator<long>>();
        private readonly IHiLoRepositoryFactory _repositoryFactory;
        private readonly IHiLoConfiguration _config;
        private Regex _entityNameValidator = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]*$");

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

        /// <summary>
        /// Gets the object which generates new unique keys for a giben entity name.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public IKeyGenerator<long> GetKeyGenerator(string entityName)
        {
            EnsureCorrectEntityName(entityName);
            lock (_lock)
            {
                if (!_keyGenerators.ContainsKey(entityName))
                    _keyGenerators.Add(entityName, CreateKeyGenerator(entityName));
                return _keyGenerators[entityName];
            }
        }

        private void EnsureCorrectEntityName(string entityName)
        {
            if (!_entityNameValidator.IsMatch(entityName) || entityName.Length > Constants.MAX_LENGTH_ENTITY_NAME)
            {
                throw new NHiloException(ErrorCodes.InvalidEntityName);
            }
        }

        private IKeyGenerator<long> CreateKeyGenerator(string entityName)
        {
            var entityConfig = _config.GetEntityConfig(entityName);
            return new HiLoGenerator(_repositoryFactory.GetRepository(entityName, _config), entityConfig != null ? entityConfig.MaxLo : _config.DefaultMaxLo);
        }
    }
}
