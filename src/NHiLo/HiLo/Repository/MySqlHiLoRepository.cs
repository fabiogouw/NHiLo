using NHiLo.HiLo.Config;
using System.Data;

namespace NHiLo.HiLo.Repository
{
    public class MySqlHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;

        public MySqlHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config, MySql.Data.MySqlClient.MySqlClientFactory.Instance)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement("CREATE TABLE IF NOT EXISTS `{0}` ( `{2}` varchar(100) NOT NULL, `{1}` BIGINT NOT NULL, PRIMARY KEY (`{2}`));", config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement("INSERT IGNORE INTO `{0}` SET {2} = {3}, {1} = 1;", config);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToGetLatestNextHiValue;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            long nextHi = (long)cmd.ExecuteScalar();
            cmd.CommandText = _sqlStatementToUpdateNextHiValue;
            cmd.ExecuteNonQuery();
            return nextHi;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToCreateRepository;
            cmd.ExecuteNonQuery();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToInitializeEntity;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }
    }
}
