using NHiLo.Common.Config;
using System.Collections.Generic;

namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Contract for a objetc that holds all configuration needed by NHiLo to work.
    /// </summary>
    public interface IHiLoConfiguration
    {
        List<IRepositoryProviderElement> Providers { get; set; }
        string ConnectionString { get; set; }
        string ProviderName { get; set; }
        string ConnectionStringId { get; }
        bool CreateHiLoStructureIfNotExists { get; }
        int DefaultMaxLo { get; }
        IEntityConfiguration GetEntityConfig(string entityName);
        string TableName { get; }
        string NextHiColumnName { get; }
        string EntityColumnName { get; }
        HiLoStorageType StorageType { get; }
        string ObjectPrefix { get; }
        int? EntityNameValidationTimeout { get; }
    }
}
