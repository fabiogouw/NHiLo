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
        private readonly string _objectPrefix;
        private readonly Regex _entityNameValidator;

        public SqlServerSequenceHiLoRepository(IHiLoConfiguration config)
            : base(config, Microsoft.Data.SqlClient.SqlClientFactory.Instance)
        {
            _objectPrefix = !string.IsNullOrWhiteSpace(config.ObjectPrefix) ? config.ObjectPrefix : "SQ_HiLo_";
            _entityNameValidator = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9_]*$", RegexOptions.None, TimeSpan.FromMilliseconds(config.EntityNameValidationTimeout.GetValueOrDefault(10)));
        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = @"DECLARE @sql NVARCHAR(MAX) = 'SELECT NEXT VALUE FOR ' + QUOTENAME(@entityName);
                                                                              EXEC sp_executesql @sql;";
            var entityNameParam = cmd.CreateParameter();
            entityNameParam.ParameterName = "entityName";
            entityNameParam.Value = BuildSequencePrefixName();
            cmd.Parameters.Add(entityNameParam);
            return (long)cmd.ExecuteScalar();
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {   
            // no need to initialize repository, each entity will have its own sequence
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
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
            entityNameParam.Value = BuildSequencePrefixName();
            cmd.Parameters.Add(entityNameParam);
            cmd.ExecuteNonQuery();
        }

        private string BuildSequencePrefixName()
        {
            string sequenceName = _objectPrefix + EntityName;
            try
            {
                if (!_entityNameValidator.IsMatch(sequenceName) || sequenceName.Length > Constants.MAX_LENGTH_ENTITY_NAME)
                {
                    throw new NHiLoException(ErrorCodes.InvalidEntityName);
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                throw new NHiLoException(ErrorCodes.EntityNameValidationTimedOut, ex);
            }
            return sequenceName;
        }
    }
}
