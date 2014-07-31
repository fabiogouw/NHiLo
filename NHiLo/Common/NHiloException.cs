using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo // this should be available at the root namespace
{
    /// <summary>
    /// A custom exception for NHilo.
    /// </summary>
    [Serializable]
    public class NHiloException : ApplicationException 
    {
        internal NHiloException(ErrorCodes errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Gives the error code that distinguishes the reason.
        /// </summary>
        public ErrorCodes ErrorCode { get; private set; }
    }
}
