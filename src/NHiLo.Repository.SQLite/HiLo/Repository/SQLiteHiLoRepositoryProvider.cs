using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    public class SQLiteHiLoRepositoryProvider : IHiLoRepositoryProvider
    {
        public string Name => "Microsoft.Data.Sqlite";

        public IHiLoRepository Build(IHiLoConfiguration config)
        {
            return new SqliteHiLoRepository(config);
        }
    }
}
