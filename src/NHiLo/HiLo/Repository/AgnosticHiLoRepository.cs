using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using NHiLo.Common;
using NHiLo.HiLo.Config;

namespace NHiLo.HiLo.Repository
{
    /// <summary>
    /// A base repository that allows the creation of DBMS NHilo repositories.
    /// </summary>
    public abstract class AgnosticHiLoRepository : IHiLoRepository
    {
        protected string _entityName;
        private IHiLoConfiguration _config;
        internal Func<string, DbProviderFactory> DbFactoryCreator { private get; set; } // for testability

        public AgnosticHiLoRepository(string entityName, IHiLoConfiguration config)
        {
            _entityName = entityName;
            _config = config;
            DbFactoryCreator = (providerName) => DbProviderFactories.GetFactory(providerName);
        }

        public void PrepareRepository()
        {
            PrepareCommandForExecutionWithTransaction(cmd =>
                {
                    if (_config.CreateHiLoStructureIfNotExists) // this prevents situations where the user doesn't have database permissions to create tables
                        CreateRepositoryStructure(cmd);
                    InitializeRepositoryForEntity(cmd);
                });
        }

        public long GetNextHi()
        {
            long result = -1;
            PrepareCommandForExecutionWithTransaction(cmd => result = GetNextHiFromDatabase(cmd));
            return result;
        }

        protected abstract long GetNextHiFromDatabase(IDbCommand cmd);

        protected abstract void CreateRepositoryStructure(IDbCommand cmd);

        protected abstract void InitializeRepositoryForEntity(IDbCommand cmd);

        protected virtual string EntityParameterName
        {
            get { return "@pEntity"; }
        }

        /// <summary>
        /// Prepare the SQL statement provided these information:
        /// {0} - table name
        /// {1} - nexthi column name
        /// {2} - entity column name
        /// {3} - parameter name
        /// </summary>
        /// <param name="rawStatement">The SQL statement which will be filled with custom information.</param>
        /// <param name="config">Object that holds the database information.</param>
        /// <returns></returns>
        protected string PrepareSqlStatement(string rawStatement, IHiLoConfiguration config)
        {
            return string.Format(rawStatement, config.TableName, config.NextHiColumnName, config.EntityColumnName, EntityParameterName);
        }

        protected virtual IDataParameter CreateEntityParameter(IDbCommand cmd, string value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = EntityParameterName;
            param.DbType = DbType.String;
            param.Direction = ParameterDirection.Input;
            param.Value = value;
            return param;
        }

        private IDbConnection CreateConnection()
        {
            string connectionString = _config.ConnectionString;
            string providerName = _config.ProviderName;
            DbConnection conn = null;
            if (!string.IsNullOrEmpty(connectionString))
            {
                var factory = DbFactoryCreator(providerName);
                conn = factory.CreateConnection();
                conn.ConnectionString = connectionString;
            }
            return conn;
        }

        private void PrepareCommandForExecutionWithTransaction(Action<IDbCommand> commandAction)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = trans;
                            commandAction.Invoke(cmd);
                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
