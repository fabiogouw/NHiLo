//using Microsoft.Extensions.Configuration;
using System.Configuration;
using NHiLo.HiLo.Config;
using System.IO;
using System.Text;
using Xunit;
using NHiLo.Common.Config.Legacy;
using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;
using FluentAssertions;

namespace NHiLo.Tests.HiLo.Config
{
    public class NETConfigConfigurationProviderTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldReadTheConfigurationsFromTheDefaultCongigurationFile()
        {

            var builder = new ConfigurationBuilder()
                .Add(new NETConfigConfigurationProvider());
            IConfiguration configuration = builder.Build();
            configuration.GetValue<string>("NHiLo:ConnectionStringId").Should().Be("DB1");

            /*var configuration = CreateConfiguration(@"{
                ""ConnectionStrings"":{
                    ""NHiLo"": ""test""
                }
            }");
            var wrapper = new ConfigurationManagerWrapper(configuration);
            Assert.Equal("test", wrapper.GetConnectionStringsSection().ConnectionStrings[0].ConnectionString);
            Assert.Equal(string.Empty, wrapper.GetConnectionStringsSection().ConnectionStrings[0].ProviderName);*/
        }

        /*private IConfiguration CreateConfiguration(string appSettings)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            return builder.Build();
        }*/
    }
}
