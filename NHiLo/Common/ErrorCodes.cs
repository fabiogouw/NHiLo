using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHiLo
{
    public enum ErrorCodes : int
    {
        /// <summary>
        /// NHilo was unable to find a connection string to connect to the repository to store
        /// the key values.
        /// </summary>
        NoConnectionStringAvailable = 1000
    }
}
