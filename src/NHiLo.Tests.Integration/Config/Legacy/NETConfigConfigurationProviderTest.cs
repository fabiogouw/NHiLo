using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NHiLo.Common.Config.Legacy;
using Xunit;

namespace NHiLo.Tests.HiLo.Config
{
    public class NetConfigConfigurationProviderTest
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void Should_ReadTheConfigurationsFromTheAppConfigFile_When_UsingTheLegacyConfigurationMode()
        {
            // HACK: the file testhost.dll.config is copied to the bin folder, during the build stage, to be loaded by the test engine
            var builder = new ConfigurationBuilder()
                .Add(new NetConfigConfigurationProvider());
            IConfiguration configuration = builder.Build();
            configuration.GetValue<string>("NHiLo:ConnectionStringId").Should().Be("DB1");
            configuration.GetValue<bool>("NHiLo:CreateHiLoStructureIfNotExists").Should().Be(true);
            configuration.GetValue<int>("NHiLo:DefaultMaxLo").Should().Be(10);
            configuration.GetValue<string>("NHiLo:StorageType").Should().Be("Sequence");
            configuration.GetValue<string>("NHiLo:ObjectPrefix").Should().Be("SQXX_");
            configuration.GetValue<int>("NHiLo:Entities:xpto1:MaxLo").Should().Be(100);
            configuration.GetValue<int>("NHiLo:Entities:xpto2:MaxLo").Should().Be(66);
        }
    }
}
