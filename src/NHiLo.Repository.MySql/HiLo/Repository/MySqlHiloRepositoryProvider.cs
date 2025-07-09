using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    public class MySqlHiloRepositoryProvider : IHiLoRepositoryProvider
    {
        public string Name => "MySql.Data.MySqlClient";

        public IHiLoRepository Build(IHiLoConfiguration config)
        {
            return new MySqlHiLoRepository(config);
        }
    }
}
