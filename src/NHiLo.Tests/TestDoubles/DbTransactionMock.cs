using System;
using System.Data;
using System.Data.Common;

namespace NHiLo.Tests.TestDoubles
{
    public class DbTransactionMock : DbTransaction
    {
        public override void Commit()
        {

        }

        protected override DbConnection DbConnection
        {
            get { throw new NotImplementedException(); }
        }

        public override IsolationLevel IsolationLevel
        {
            get { throw new NotImplementedException(); }
        }

        public override void Rollback()
        {

        }
    }
}
