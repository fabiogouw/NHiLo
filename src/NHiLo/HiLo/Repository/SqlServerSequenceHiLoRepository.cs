using NHiLo.Common;
using NHiLo.HiLo.Config;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// NHiLo's repository implementation for Microsoft SQL Server.
    /// </summary>
    public class SqlServerSequenceHiLoRepository : AgnosticHiLoRepository
    {
        private readonly string _sqlStatementToSelectAndUpdateNextHiValue = @"DECLARE @sql NVARCHAR(MAX) = 'SELECT NEXT VALUE FOR ' + QUOTENAME(@entityName);
                                                                              EXEC sp_executesql @sql;";
        private readonly string _objectPrefix = "SQ_HiLo_";
        private readonly Regex _entityNameValidator = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9_]*$", RegexOptions.None, TimeSpan.FromMilliseconds(10));

        public SqlServerSequenceHiLoRepository(IHiLoConfiguration config)
            : base(config, Microsoft.Data.SqlClient.SqlClientFactory.Instance)
        {
            if (!string.IsNullOrEmpty(config.ObjectPrefix))
            {
                _objectPrefix = config.ObjectPrefix;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated within the method.")]
        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            EnsureCorrectSequencePrefixName();
            cmd.CommandText = _sqlStatementToSelectAndUpdateNextHiValue;
            var entityNameParam = cmd.CreateParameter();
            entityNameParam.ParameterName = "entityName";
            entityNameParam.Value = _objectPrefix + EntityName;
            cmd.Parameters.Add(entityNameParam);
            return (long)cmd.ExecuteScalar();
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {   
            // no need to initialize repository, each entity will have tis own sequence
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated within the method.")]
        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            EnsureCorrectSequencePrefixName();
            cmd.CommandText = @"
            DECLARE @sql NVARCHAR(MAX) = 'IF NOT EXISTS(SELECT 1 FROM sys.sequences WHERE name = ''' + QUOTENAME(@entityName) + ''')
                                          BEGIN
	                                          CREATE SEQUENCE [dbo].' + QUOTENAME(@entityName) + ' START WITH 1 INCREMENT BY 1;
	                                          SELECT 1;
                                          END
                                          ELSE
	                                          SELECT 0;'
            EXEC sp_executesql @sql;";
            var entityNameParam = cmd.CreateParameter();
            entityNameParam.ParameterName = "entityName";
            entityNameParam.Value = _objectPrefix + EntityName;
            cmd.Parameters.Add(entityNameParam);
            cmd.ExecuteNonQuery();
        }

        private void EnsureCorrectSequencePrefixName()
        {
            if (!_entityNameValidator.IsMatch(_objectPrefix) || _objectPrefix.Length > Constants.MAX_LENGTH_ENTITY_NAME)
            {
                throw new NHiLoException(ErrorCodes.InvalidSequencePrefixName);
            }
        }
    }
}
