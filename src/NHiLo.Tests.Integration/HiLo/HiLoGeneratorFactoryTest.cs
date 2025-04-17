using FluentAssertions;
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

            [Fact]
            [Trait("Category", "Integration")]
            public void Should_ThrowEntityNameValidationTimedOutException_When_UsingAnInvalidEntityNameThatExceedsTheRegexValidationTimeout()
            {
                // Arrange
                var factory = new HiLoGeneratorFactory();   // it's intended to use a legacy code here
                string complexEntityName = new string('a', 99999) + "!";
                try
                {
                    // Act
                    var generator = factory.GetKeyGenerator(complexEntityName);
                }
                catch (NHiLoException ex)
                {
                    // Assert
                    ex.ErrorCode.Should().Be(ErrorCodes.EntityNameValidationTimedOut);
                }
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Should_ThrowExceptionInvalidEntityName_When_UsingAnInvalidEntityNameThatExceedsTheRegexValidationTimeout2()
            {
                // Arrange
                var builder = new ConfigurationBuilder();
                builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes($@"{{
                    ""NHiLo"":{{
                        ""EntityNameValidationTimeout"": ""1000000""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""anything"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}")));
                var factory = new HiLoGeneratorFactory(builder.Build());
                string complexEntityName = new string('a', 99999) + "!";
                try
                {
                    // Act
                    var generator = factory.GetKeyGenerator(complexEntityName);
                }
                catch (NHiLoException ex)
                {
                    // Assert
                    ex.ErrorCode.Should().Be(ErrorCodes.InvalidEntityName);
                }
            }
        }
    }
}
