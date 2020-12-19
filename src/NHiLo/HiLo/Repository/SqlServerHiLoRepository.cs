using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHiLo.Properties;
using System.Data;
using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// NHilo's repository implementation for Microsoft SQL Server.
    /// </summary>
    public class SqlServerHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToSelectAndUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;

        public SqlServerHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config, Microsoft.Data.SqlClient.SqlClientFactory.Instance)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToSelectAndUpdateNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3};UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3};", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement(Resources.SqlServerCreateStructure, config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement(Resources.SqlServerInitializeEntity, config);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToSelectAndUpdateNextHiValue;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
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
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }
    }
}
