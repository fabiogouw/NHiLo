using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo.Common
{
    public class NHiloException : ApplicationException 
    {
        public NHiloException(ErrorCodes errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        public ErrorCodes ErrorCode { get; private set; }
    }
}
