using System;

namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Contract for a objetc that holds all configuration needed by NHiLo to work.
    /// </summary>
    public interface IHiLoConfiguration
    {
        string ConnectionString { get; set; }
        string ProviderName { get; set; }
        string ConnectionStringId { get; }
        bool CreateHiLoStructureIfNotExists { get; }
        int DefaultMaxLo { get; }
        IEntityConfiguration GetEntityConfig(string entityName);
        string TableName{ get; }
        string NextHiColumnName{ get; }
        string EntityColumnName{ get; }
    }
}
