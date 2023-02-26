using Microsoft.Extensions.Configuration;
using NHiLo.HiLo;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace NHiLo.Tests.HiLo
{
    public class HiLoGeneratorFactoryTest
    {
        public class GetKeyGenerator
        {
            [Fact]
            [Trait("Category", "Integration")]
            public void Should_ReturnAnInstanceOfHiLoGenerator_When_UsingTheLegacyConfigurationMode()
            {
                // Arrange
                var factory = new HiLoGeneratorFactory();   // it's intended to use a legacy code here
                // Act
                var generator = factory.GetKeyGenerator("dummy");
                // Assert
                Assert.IsAssignableFrom<HiLoGenerator>(generator);
            }
        }
    }
}
