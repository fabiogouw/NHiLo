using NHiLo.HiLo;
using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    public class OracleHiloRepositoryProvider : IHiLoRepositoryProvider
    {
        public string Name => "System.Data.OracleClient";

        public IHiLoRepository Build(IHiLoConfiguration config)
        {
            return new OracleHiLoRepository(config);
        }
    }
}
