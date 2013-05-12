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
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;
    
        public OracleHiLoRepository(string entityName, IHiLoConfiguration config)
            : base(entityName, config)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement("DECLARE vCOUNT NUMBER; " +
                                "BEGIN " +
                                "SELECT COUNT(*) INTO vCOUNT FROM USER_TABLES WHERE TABLE_NAME = '{0}'; " +
                                "IF vCOUNT = 0 THEN " +
                                "  EXECUTE IMMEDIATE 'CREATE TABLE {0} ({2} VARCHAR2(100) NOT NULL, {1} NUMBER(19) NOT NULL, CONSTRAINT PK_{0} PRIMARY KEY ({2}))'; " +
                                "END IF; " +
                                "END;", config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement("DECLARE vCOUNT NUMBER; " +
                    "BEGIN " +
                    "SELECT COUNT(*) INTO vCOUNT FROM {0} WHERE {2} = {3}; " +
                    "IF vCOUNT = 0 THEN " +
                    "  INSERT INTO {0} ({2}, {1}) VALUES ({3}, 1); " +
                    "END IF; " +
                    "END;", config);
        }

        protected override long GetNextHiFromDatabase(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToGetLatestNextHiValue;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            long nextHi = Convert.ToInt64(cmd.ExecuteScalar());
            cmd.CommandText = _sqlStatementToUpdateNextHiValue;
            cmd.ExecuteNonQuery();
            return nextHi;
        }

        protected override void CreateRepositoryStructure(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToCreateRepository;
            cmd.ExecuteNonQuery();
        }

        protected override void InitializeRepositoryForEntity(IDbCommand cmd)
        {
            cmd.CommandText = _sqlStatementToInitializeEntity;
            cmd.Parameters.Add(CreateEntityParameter(cmd, _entityName));
            cmd.ExecuteNonQuery();
        }

        protected override string EntityParameterName
        {
            get { return ":pEntity"; }
        }
    }
}
