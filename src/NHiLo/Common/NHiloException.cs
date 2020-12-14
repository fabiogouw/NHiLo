using NHiLo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            : base(GetMessageForErrorCode(errorCode))
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Gives the error code that distinguishes the reason.
        /// </summary>
        public ErrorCodes ErrorCode { get; private set; }

        private static string GetMessageForErrorCode(ErrorCodes errorCode)
        {
            return errorCode.GetDescription();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ErrorCode", ErrorCode);
        }
    }
}
