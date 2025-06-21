namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Represents the configuration for each entity (aka. table) and its max lo value.
    /// </summary>
    public class RepositoryProviderElement : IRepositoryProviderElement
    {
        public virtual string Name
        {
            get; internal set;
        }

        public virtual string Type
        {
            get; internal set;
        }
    }
}
