using NHiLo.HiLo.Config;
using System.Configuration;

namespace NHiLo.Common.Config
{
    public interface IConfigurationManager
    {
        IHiLoConfiguration GetKeyGeneratorConfigurationSection();
        ConnectionStringsSection GetConnectionStringsSection();
    }
}
