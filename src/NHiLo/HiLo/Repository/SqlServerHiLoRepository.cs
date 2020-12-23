using NHiLo.HiLo.Config;
using System.Data;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// NHiLo's repository implementation for Microsoft SQL Server.
    /// </summary>
    public class SqlServerHiLoRepository : AgnosticHiLoRepository
    {
        private const string SQLSERVERCREATESTRUCTURE = @"
            IF NOT EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE NAME = '{0}' AND TYPE = 'U')
            BEGIN
                CREATE TABLE [dbo].[{0}](
                [{2}] [nvarchar](100) NOT NULL,
                [{1}] [bigint] NOT NULL,
                    CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                    (
                        [{2}] ASC
                    )
                );
                SELECT 1;
            END
            ELSE
                SELECT 0;";
        private const string SQLSERVERINITIALIZEENTITY = @"
            SET NOCOUNT ON;
            IF NOT EXISTS(SELECT 1 FROM {0} WHERE {2} = {3})
            BEGIN
                INSERT INTO {0} VALUES ({3}, 1);
            END
        ";
        private string _sqlStatementToSelectAndUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;

        public SqlServerHiLoRepository(IHiLoConfiguration config)
            : base(config, Microsoft.Data.SqlClient.SqlClientFactory.Instance)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToSelectAndUpdateNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3};UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3};", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement(SQLSERVERCREATESTRUCTURE, config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement(SQLSERVERINITIALIZEENTITY, config);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToSelectAndUpdateNextHiValue;
            cmd.Parameters.Add(CreateEntityParameter(cmd, EntityName));
            return (long)cmd.ExecuteScalar();
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
            cmd.Parameters.Add(CreateEntityParameter(cmd, EntityName));
            cmd.ExecuteNonQuery();
        }
    }
}
