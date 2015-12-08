﻿using NHiLo.Guid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo // this should be available at the root namespace
{
    /// <summary>
    /// Factory that creates <see cref="IKeyGeneratorFactory"/> for client usage.
    /// </summary>
    public class GuidGeneratorFactory : IKeyGeneratorFactory<string>
    {
        public IKeyGenerator<string> GetKeyGenerator(string entityName)
        {
            return new GuidGenerator();
        }
    }
}