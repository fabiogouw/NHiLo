using System;
using System.Collections.Generic;
using System.Text;

namespace NHiLo.HiLo.Config
{
    public interface IRepositoryProviderElement
    {
        string Name { get; }
        string Type { get; }
    }
}
