using System;
using NHiLo.HiLo.Config;

namespace NHiLo.Common
{
    public interface IKeyGeneratorConfiguration
    {
        IHiLoConfiguration HiloKeyGenerator { get; }
    }
}
