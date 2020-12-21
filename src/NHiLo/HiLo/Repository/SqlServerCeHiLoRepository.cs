using NHiLo.HiLo.Config;
using System.Data;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// NHilo's repository implementation for Microsoft SQL Server CE.
    /// </summary>
    public class SqlServerCeHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToCheckIfNHilosTableExists;
        private string _sqlStatementToCreateNHiloTable;
        private string _sqlStatementToCheckIfEntityExists;
        private string _sqlStatementToInsertNewEntityToNHilosTable;

        public SqlServerCeHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config, System.Data.SQLite.SQLiteFactory.Instance)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToCheckIfNHilosTableExists = PrepareSqlStatement("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}' AND TABLE_TYPE = 'TABLE'", config);
            _sqlStatementToCreateNHiloTable = PrepareSqlStatement("CREATE TABLE {0} ({2} NVARCHAR(100) PRIMARY KEY NOT NULL, {1} BIGINT NOT NULL)", config);
            _sqlStatementToCheckIfEntityExists = PrepareSqlStatement("SELECT 1 FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToInsertNewEntityToNHilosTable = PrepareSqlStatement("INSERT INTO {0} ({2}, {1}) VALUES ({3}, 1)", config);
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
            cmd.CommandText = _sqlStatementToCheckIfNHilosTableExists;
            var existsTable = cmd.ExecuteScalar() != null;
            if (!existsTable)
            {
                cmd.CommandText = _sqlStatementToCreateNHiloTable;
                cmd.ExecuteNonQuery();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToCheckIfEntityExists;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            var entityAlreadyInitialized = cmd.ExecuteScalar() != null;
            if (!entityAlreadyInitialized)
            {
                cmd.CommandText = _sqlStatementToInsertNewEntityToNHilosTable;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
