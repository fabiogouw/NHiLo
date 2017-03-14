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
    public class SqlServerSequenceHiLoRepository : AgnosticHiLoRepository
    {
        private readonly string _sqlStatementToSelectAndUpdateNextHiValue = @"SELECT NEXT VALUE FOR [dbo].[{0}{1}];";
        private readonly string _objectPrefix = "SQ_HiLo_";

        public SqlServerSequenceHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config)
        {
            if (!string.IsNullOrEmpty(config.ObjectPrefix))
            {
                _objectPrefix = config.ObjectPrefix;
            }
        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = string.Format(_sqlStatementToSelectAndUpdateNextHiValue, _objectPrefix, _entityName);
            return (long)cmd.ExecuteScalar();
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            // no need to initialize repository, each entity will have tis own sequence
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = string.Format(@"
            IF NOT EXISTS(SELECT 1 FROM sys.sequences WHERE name = '{0}{1}')
            BEGIN
	            CREATE SEQUENCE [dbo].[{0}{1}] START WITH 1 INCREMENT BY 1;
	            SELECT 1;
            END
            ELSE
	            SELECT 0;", _objectPrefix, _entityName);
            cmd.ExecuteNonQuery();
        }
    }
}
