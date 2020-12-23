using System;
using System.Data.Common;

namespace NHiLo.Tests.TestDoubles
{
    public class DbCommandMock : DbCommand
    {
        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; }

        public override System.Data.CommandType CommandType { get; set; }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { throw new NotImplementedException(); }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible { get; set; }

        protected override DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        public override System.Data.UpdateRowSource UpdatedRowSource { get; set; }
    }
}
