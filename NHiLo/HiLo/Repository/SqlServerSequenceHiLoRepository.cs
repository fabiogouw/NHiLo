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
        private readonly string _sqlStatementToSelectAndUpdateNextHiValue = @"SELECT NEXT VALUE FOR [dbo].[HiLo{0}];";

        public SqlServerSequenceHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config)
        {
        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = string.Format(_sqlStatementToSelectAndUpdateNextHiValue, "_entityName");
            return (long)cmd.ExecuteScalar();
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            // no need to initialize repository, each entity will have tis own sequence
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = @"
            IF NOT EXISTS(SELECT 1 FROM sys.sequences WHERE name = '{0}')
            BEGIN
	            CREATE SEQUENCE [dbo].[HiLo{0}] START WITH 1 INCREMENT BY 1;
	            SELECT 1;
            END
            ELSE
	            SELECT 0;";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }
    }
}
