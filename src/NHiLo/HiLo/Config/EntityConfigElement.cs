namespace NHiLo.HiLo.Config
{
    /// <summary>
    /// Represents the configuration for each entity (aka. table) and its max lo value.
    /// </summary>
    public class EntityConfigElement : IEntityConfiguration
    {
        public virtual string Name
        {
            get; internal set;
        }

        public virtual int MaxLo
        {
            get; internal set;
        }
    }
}
