using NHiLo.HiLo.Config;

namespace NHiLo.Tests.TestDoubles.Config
{
    public class EntityConfigurationStub : IEntityConfiguration
    {
        public string Name { get; set; }

        public int MaxLo { get; set; }
    }
}
