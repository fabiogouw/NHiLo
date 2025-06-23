using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// Repository provider for Microsoft SQL Server.
    /// </summary>
    public class SqlServerSequenceHiLoRepositoryProvider : IHiLoRepositoryProvider
    {
        public string Name => "Microsoft.Data.SqlClient.Sequence";

        public IHiLoRepository Build(IHiLoConfiguration config)
        {
            return new SqlServerSequenceHiLoRepository(config);
        }
    }
}
