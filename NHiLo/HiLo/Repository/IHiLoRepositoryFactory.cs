using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    public interface IHiLoRepositoryFactory
    {
        IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config);
    }
}
