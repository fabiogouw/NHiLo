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
        public SqlServerHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config)
        {

        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = "SELECT NEXT_HI FROM NHILO WHERE ENTITY = @pEntity;UPDATE NHILO SET NEXT_HI = NEXT_HI + 1 WHERE ENTITY = @pEntity;";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            return (long)cmd.ExecuteScalar();
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            cmd.CommandText = Resources.SqlServerCreateStructure;
            cmd.ExecuteNonQuery();
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = Resources.SqlServerInitializeEntity;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }
    }
}
