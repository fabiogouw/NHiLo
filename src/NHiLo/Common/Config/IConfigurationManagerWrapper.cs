using NHiLo.HiLo.Config;
using System.Configuration;

namespace NHiLo.Common.Config
{
    public interface IConfigurationManagerWrapper
    {
        IHiLoConfiguration GetKeyGeneratorConfigurationSection();
        ConnectionStringsSection GetConnectionStringsSection();
    }
}
