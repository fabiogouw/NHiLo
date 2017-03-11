using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("No connection strings available. NHiLo can't identify the database to use.")]
        NoConnectionStringAvailable = 1000,
        /// <summary>
        /// Entity name has invalid characters (not only letters and numbers).
        /// </summary>
        [Description("An entity name must only contains letters and numbers, starting with a letter.")]
        InvalidEntityName = 1001
    }
}
