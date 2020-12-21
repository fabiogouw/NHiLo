using NHiLo.HiLo.Config;
using System;

namespace NHiLo.HiLo.Repository
{
    public interface IHiLoRepositoryFactory
    {
        IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config);
        void RegisterRepository(string providerName, Func<IHiLoRepository> funcCreateRepository);
    }
}
