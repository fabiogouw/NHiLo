using System;

namespace NHiLo.HiLo.Config
{
    public interface IEntityConfiguration
    {
        bool Name { get; }
        int MaxLo { get; }
    }
}
