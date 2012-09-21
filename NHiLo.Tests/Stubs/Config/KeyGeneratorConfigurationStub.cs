using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Common;
using NHiLo.HiLo.Config;

namespace NHiLo.Tests.Stubs.Config
{
    public class KeyGeneratorConfigurationStub : IKeyGeneratorConfiguration
    {
        public IHiLoConfiguration HiloKeyGenerator
        {
            get { return HiloKeyGeneratorFunction.Invoke(); }
        }

        public Func<IHiLoConfiguration> HiloKeyGeneratorFunction { get; set; }
    }
}
