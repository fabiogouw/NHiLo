using NHiLo.HiLo.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHiLo.HiLo.Repository
{
    public interface IHiLoRepositoryProvider
    {
        string Name { get; }
        IHiLoRepository Build(IHiLoConfiguration config);
    }
}
