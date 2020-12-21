using System;

namespace NHiLo.HiLo
{
    /// <summary>
    /// Generates a new key based on HiLo algorithm.
    /// </summary>
    public class HiLoGenerator : IKeyGenerator<long>
    {
        private static readonly object _lock = new object(); // for handling multiple calls at the same time
        private readonly IHiLoRepository _repository;
        private long _currentHi;
        private readonly int _maxLo;
        private int _currentLo = int.MaxValue;  // starts with the maximum value to ensure the repository's first call

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="repository">An implementation of the repository used to keep the high values.</param>
        /// <param name="maxLo">The value used as the low part of the key.</param>
        public HiLoGenerator(IHiLoRepository repository, int maxLo)
        {
            if (maxLo <= 0)
            {
                throw new ArgumentException("The value of 'maxLo' must be greater than zero.");
            }
            _repository = repository ?? throw new ArgumentException("An valid instance of IHiLoRepository must be provided.");
            _maxLo = maxLo;
        }

        /// <summary>
        /// Get as unique value to be used as a primary key.
        /// </summary>
        /// <returns>Unique long value.</returns>
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
