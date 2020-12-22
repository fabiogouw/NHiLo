using NHiLo.HiLo.Config;
using System.Collections.Generic;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// An in-memory implementation for testing purpuse.
    /// </summary>
    public class InMemoryHiloRepository : IHiLoRepository
    {
        private static readonly IDictionary<string, long> _repository = new Dictionary<string, long>();

        private string _entityName;
        public long GetNextHi()
        {
            lock (_repository)
            {
                long currentValue = _repository[_entityName];
                _repository[_entityName] = currentValue + 1;
                return _repository[_entityName];
            }
        }

        public void PrepareRepository(string entityName)
        {
            _entityName = entityName;
            _repository.Add(entityName, 0);
        }
    }
}
