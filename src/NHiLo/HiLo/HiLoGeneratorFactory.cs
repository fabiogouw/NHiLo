using Microsoft.Extensions.Configuration;
using NHiLo.Common;
using NHiLo.Common.Config.Legacy;
using NHiLo.HiLo;
using NHiLo.HiLo.Config;
using NHiLo.HiLo.Repository;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace NHiLo // this should be available at the root namespace
{
    /// <summary>
    /// Factory that creates <see cref="IKeyGeneratorFactory"/> for client usage.
    /// </summary>
    public class HiLoGeneratorFactory : IKeyGeneratorFactory<long>
    {
        // When instantiated, key generators are stored in a static field. That's how NHiLo keeps the id generation globally per AppDomain.
        private readonly static ConcurrentDictionary<string, IKeyGenerator<long>> _keyGenerators = new ConcurrentDictionary<string, IKeyGenerator<long>>();
        private readonly IHiLoRepositoryFactory _repositoryFactory;
        private readonly IHiLoConfiguration _config;
        private readonly Regex _entityNameValidator;

        [Obsolete("For legacy compatibility only (.NET Framework). Newer versions like .NET Core and .NET 5 should use the constructor that receives an IConfiguration parameter.")]
        public HiLoGeneratorFactory() :
            this(null)
        {

        }

        public HiLoGeneratorFactory(IConfiguration configuration)
        {
            if (configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .Add(new NetConfigConfigurationProvider())
                    .AddEnvironmentVariables();
                configuration = builder.Build();
            }
            _config = new HiLoConfigurationBuilder(new ConfigurationManagerWrapper(configuration)).Build();
            _entityNameValidator = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]*$", RegexOptions.None, TimeSpan.FromMilliseconds(_config.EntityNameValidationTimeout.GetValueOrDefault(10)));
            _repositoryFactory = new HiLoRepositoryFactory();
        }

        /// <summary>
        /// Gets the object which generates new unique keys for a given entity name.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public IKeyGenerator<long> GetKeyGenerator(string entityName)
        {
            EnsureCorrectEntityName(entityName);
            lock (_keyGenerators)
            {
                if (!_keyGenerators.ContainsKey(entityName))
                    _keyGenerators.TryAdd(entityName, CreateKeyGenerator(entityName));
                return _keyGenerators[entityName];
            }
        }

        private void EnsureCorrectEntityName(string entityName)
        {
            try
            {
                if (!_entityNameValidator.IsMatch(entityName) || entityName.Length > Constants.MAX_LENGTH_ENTITY_NAME)
                {
                    throw new NHiLoException(ErrorCodes.InvalidEntityName);
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                throw new NHiLoException(ErrorCodes.EntityNameValidationTimedOut, ex);
            }

        }

        private IKeyGenerator<long> CreateKeyGenerator(string entityName)
        {
            var entityConfig = _config.GetEntityConfig(entityName);
            var repository = _repositoryFactory.GetRepository(entityName, _config);
            return new HiLoGenerator(repository, entityConfig.MaxLo);
        }
    }
}
