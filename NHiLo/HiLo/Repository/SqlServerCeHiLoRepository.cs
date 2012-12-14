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
    /// NHilo's repository implementation for Microsoft SQL Server CE.
    /// </summary>
    public class SqlServerCeHiLoRepository : AgnosticHiLoRepository
    {
        public SqlServerCeHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config)
        {

        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = "SELECT NEXT_HI FROM NHILO WHERE ENTITY = @pEntity";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            long nextHi = (long)cmd.ExecuteScalar();
            cmd.CommandText = "UPDATE NHILO SET NEXT_HI = NEXT_HI + 1 WHERE ENTITY = @pEntity";
            cmd.ExecuteNonQuery();
            return nextHi;
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            cmd.CommandText = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NHILO' AND TABLE_TYPE = 'TABLE'";
            var existsTable = cmd.ExecuteScalar() != null;
            if(!existsTable)
            {
                cmd.CommandText = "CREATE TABLE NHILO (ENTITY NVARCHAR(100) PRIMARY KEY NOT NULL, NEXT_HI BIGINT NOT NULL)";
                cmd.ExecuteNonQuery();
            }
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = "SELECT 1 FROM NHILO WHERE ENTITY = @pEntity";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            var entityAlreadyInitialized = cmd.ExecuteScalar() != null;
            if (!entityAlreadyInitialized)
            {
                cmd.CommandText = "INSERT INTO NHILO (ENTITY, NEXT_HI) VALUES (@pEntity, 1)";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
