using NHiLo.Common;
using NHiLo.HiLo.Config;
using System;

namespace NHiLo.Tests.TestDoubles.Config
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
