using System;
using System.Collections.Generic;

namespace NHiLo.Common
{
    /// <summary>
    /// Represents the contract used to create objects that generate keys.
    /// </summary>
    /// <typeparam name="T">The type of the key (long, string, etc).</typeparam>
    public interface IKeyGeneratorFactory<T>
    {
        IKeyGenerator<T> GetKeyGenerator(string entityName);
    }
}
