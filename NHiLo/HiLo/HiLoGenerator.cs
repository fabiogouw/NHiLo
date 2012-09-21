using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Common;
using System.Diagnostics.Contracts;

namespace NHiLo.HiLo
{
    /// <summary>
    /// Generates a new key based on HiLo algorithm.
    /// </summary>
    public class HiLoGenerator : IKeyGenerator<long>
    {
        private static object _lock = new object(); // for handling multiple calls at the same time
        private readonly IHiLoRepository _repository;
        private long _currentHi;
        private readonly int _maxLo;
        private int _currentLo = int.MaxValue;  // starts with the maximum value to ensure the repository's first call

        public HiLoGenerator(IHiLoRepository repository, int maxLo)
        {
            Contract.Requires<ArgumentException>(repository != null, "An valid instance of IHiLoRepository must be provided.");
            Contract.Requires<ArgumentException>(maxLo > 0, "The value of 'maxLo' must be greater than zero.");
            _repository = repository;
            _maxLo = maxLo;
        }

        public long GetKey()
        {
            lock (_lock)
            {
                if (_currentLo >= _maxLo)
                {
                    _currentHi = _repository.GetNextHi();
                    _currentLo = 0;
                }
                long result = _currentHi * _maxLo + _currentLo;
                _currentLo++;
                return result;
            }
        }
    }
}
