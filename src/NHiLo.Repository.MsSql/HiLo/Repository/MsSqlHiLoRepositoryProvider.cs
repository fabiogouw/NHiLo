using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// Repository provider for Microsoft SQL Server.
    /// </summary>
    public class MsSqlHiLoRepositoryProvider : IHiLoRepositoryProvider
    {
        public string Name => "Microsoft.Data.SqlClient";

        public IHiLoRepository Build(IHiLoConfiguration config)
        {
            return new MsSqlHiLoRepository(config);
        }
    }
}
