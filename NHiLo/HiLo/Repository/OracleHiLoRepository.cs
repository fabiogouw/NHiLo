using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    public class OracleHiLoRepository : AgnosticHiLoRepository
    {
        public OracleHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config)
        {

        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = "SELECT NEXT_HI FROM NHILO WHERE ENTITY = @pEntity";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            var nextHi = (long)cmd.ExecuteScalar();
            cmd.CommandText = "UPDATE NHILO SET NEXT_HI = NEXT_HI + 1 WHERE ENTITY = @pEntity";
            cmd.ExecuteNonQuery();
            return nextHi;
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            cmd.CommandText =   "DECLARE vCOUNT NUMBER;" + 
                                "BEGIN" + 
                                "SELECT COUNT(*) INTO vCOUNT FROM USER_TABLES WHERE TABLE_NAME = 'NHILO';" + 
                                "IF vCOUNT = 0 THEN" + 
                                "  EXECUTE IMMEDIATE 'CREATE TABLE NHILO (ENTITY VARCHAR2(100) NOT NULL, NEXT_HI NUMBER(19) NOT NULL, CONSTRAINT PK_NHILO PRIMARY KEY (ENTITY))';" + 
                                "END IF;" + 
                                "END;";
            cmd.ExecuteNonQuery();
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = "DECLARE vCOUNT NUMBER;" +
                    "BEGIN" +
                    "SELECT COUNT(*) INTO vCOUNT FROM NHILO WHERE ENTITY = @pEntity;" +
                    "IF vCOUNT = 0 THEN" +
                    "  INSERT INTO NHILO (ENTITY, NEXT_HI) VALUES (@pEntity, 1);" +
                    "END IF;" +
                    "END;";
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }
    }
}
