using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo // this should be available at the root namespace
{
    public interface IKeyGenerator<T>
    {
        /// <summary>
        /// Creates a unique key for a given entity.
        /// </summary>
        /// <returns>An unique key.</returns>
        T GetKey();
    }
}
