namespace NHiLo.Common.Config
{
    /// <summary>
    /// Holds the types of storage that can be used to keep record of the Hi values within the database.
    /// </summary>
    /// <remarks>No all DBMS can implement all these storage types.</remarks>
    public enum HiLoStorageType
    {
        /// <summary>
        /// Store Hi values in tables.
        /// </summary>
        Table = 0,
        /// <summary>
        /// Store Hi values using sequences (Oracle, SQL Server, etc.).
        /// </summary>
        Sequence = 1
    }
}
