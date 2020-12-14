using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace NHiLo.Tests.TestDoubles
{
    public class DbConnectionMock : DbConnection
    {

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new DbTransactionMock();
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {

        }

        public override string ConnectionString { get; set; }

        protected override DbCommand CreateDbCommand()
        {
            return new DbCommandMock();
        }

        public override string DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public override string Database
        {
            get { throw new NotImplementedException(); }
        }

        public override void Open()
        {

        }

        public override string ServerVersion
        {
            get { throw new NotImplementedException(); }
        }

        public override ConnectionState State
        {
            get { throw new NotImplementedException(); }
        }
    }
}
