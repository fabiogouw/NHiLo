using NHiLo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo // this should be available at the root namespace
{
    public static class KeyGeneratorExtensions
    {
        /// <summary>
        /// Returns the unique value as a int value.
        /// </summary>
        /// <param name="generator">An object that produces new key values.</param>
        /// <returns>Unique int value.</returns>
        public static int GetKeyAsInt(this IKeyGenerator<long> generator)
        {
            return (int)generator.GetKey();
        }
    }
}
