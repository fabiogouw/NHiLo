using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    public class MySqlHiLoRepository : AgnosticHiLoRepository
    {
        public MySqlHiLoRepository(string entityName, IHiLoConfiguration config)
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
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS `NHILO` ( `ENTITY` varchar(100) NOT NULL, `NEXT_HI` BIGINT NOT NULL, PRIMARY KEY  (`ENTITY`));";
            cmd.ExecuteNonQuery();
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = "INSERT IGNORE INTO `NHILO` SET ENTITY = @pEntity, NEXT_HI = 1;";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }
    }
}
