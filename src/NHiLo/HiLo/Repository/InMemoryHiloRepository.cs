using NHiLo.HiLo.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHiLo.HiLo.Repository
{
    public class InMemoryHiloRepository : IHiLoRepository
    {
        private static IDictionary<string, long> _repository = new Dictionary<string, long>();

        private string _entityName;
        public InMemoryHiloRepository(string entityName, IHiLoConfiguration config)
        {
            _entityName = entityName;
        }

        public long GetNextHi()
        {
            lock (_repository)
            {
                long currentValue = _repository[_entityName];
                _repository[_entityName] = currentValue + 1;
                return _repository[_entityName];
            }
        }

        public void PrepareRepository()
        {
            _repository.Add(_entityName, 0);
        }
    }
}
