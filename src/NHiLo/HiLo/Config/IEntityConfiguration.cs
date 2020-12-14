using System;

namespace NHiLo.HiLo.Config
{
    public interface IEntityConfiguration
    {
        string Name { get; }
        int MaxLo { get; }
    }
}
