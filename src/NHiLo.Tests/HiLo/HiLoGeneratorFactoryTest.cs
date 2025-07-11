﻿using FluentAssertions;
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
            [Trait("Category", "Unit")]
            public void ShouldReturnAnInstanceOfHiLoGenerator()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator = factory.GetKeyGenerator("dummy");
                // Assert
                Assert.IsType<HiLoGenerator>(generator);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldReturnTheSameInstanceForTheSameEntity()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator1 = factory.GetKeyGenerator("dummy");
                var generator2 = factory.GetKeyGenerator("dummy");
                // Assert
                Assert.Same(generator1, generator2);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldReturnTheSameInstanceForTheSameEntityAndForDifferentFactories()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory1 = new HiLoGeneratorFactory(mockConfig);
                var factory2 = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator1 = factory1.GetKeyGenerator("dummy");
                var generator2 = factory2.GetKeyGenerator("dummy");
                // Assert
                Assert.Same(generator1, generator2);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldReturnDifferentInstancesForDifferentEntities()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator1 = factory.GetKeyGenerator("dummy1");
                var generator2 = factory.GetKeyGenerator("dummy2");
                // Assert
                Assert.NotSame(generator1, generator2);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldReturnDifferentInstancesForDifferentEntitiesAndForDifferentFactories()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory1 = new HiLoGeneratorFactory(mockConfig);
                var factory2 = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator1 = factory1.GetKeyGenerator("dummy1");
                var generator2 = factory2.GetKeyGenerator("dummy2");
                // Assert
                Assert.NotSame(generator1, generator2);
            }
            /*
            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldCreateGeneratorGettingTheDefaultMaxLoFromConfig()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator = factory.GetKeyGenerator("dummy4");
                generator.ge
                // Assert
                Assert.Equal(1, chamadasEntityDummy);    // HACK: there's a problem with Moq where I can't verify if p => p["dummy"] was called
                mockConfig.VerifyGet(p => p.DefaultMaxLo, Times.Once());
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldCreateGeneratorGettingTheMaxLoFromSpecificEntityInConfig()
            {
                // Arrange
                var mockConfig = new Mock<IHiLoConfiguration>();
                var mockEntity1 = new Mock<IEntityConfiguration>();
                mockConfig.Setup(m => m.GetEntityConfig(It.Is<string>(p => p == "dummy3"))).Returns(mockEntity1.Object);
                mockEntity1.SetupGet(p => p.MaxLo).Returns(20);
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator = factory.GetKeyGenerator("dummy3");
                // Assert
                mockEntity1.VerifyGet(p => p.MaxLo, Times.Once());
                mockConfig.VerifyGet(p => p.DefaultMaxLo, Times.Never());
            }
            */
            [Fact]
            [Trait("Category", "Unit")]
            public void Should_RaiseException_When_EntityNameStartsWithANumber()
            {
                RaiseExceptionIfEntityNameIsInvalid("123name");
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void Should_RaiseException_When_EntityNameContainsSpaces()
            {
                RaiseExceptionIfEntityNameIsInvalid("n ame");
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void Should_RaiseException_When_EntityNameContainsSingleQuotes()
            {
                RaiseExceptionIfEntityNameIsInvalid("name'");
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void Should_RaiseException_When_EntityNameHasMoreThan100Chars()
            {
                RaiseExceptionIfEntityNameIsInvalid("namesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnames1");
            }

            private static void RaiseExceptionIfEntityNameIsInvalid(string entityName)
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                try
                {
                    factory.GetKeyGenerator(entityName);
                }
                catch (NHiLoException ex)
                {
                    // Assert
                    Assert.Equal(ErrorCodes.InvalidEntityName, ex.ErrorCode);
                }
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void Should_RaiseException_When_NoProviderNameHasBeenSupplied()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithNoConfigurationForNHiLoAndOnlyOneConnectionStringWithNoProviderName();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                try
                {
                    var generator = factory.GetKeyGenerator("dummyNoProviderName");
                }
                catch (NHiLoException ex)
                {
                    // Assert
                    Assert.Equal(ErrorCodes.NoProviderName, ex.ErrorCode);
                }
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void Should_ThrowEntityNameValidationTimedOutException_When_UsingAnInvalidEntityNameThatExceedsTheRegexValidationTimeout()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithNoConfigurationForNHiLoAndOnlyOneConnectionStringWithNoProviderName();
                var factory = new HiLoGeneratorFactory(mockConfig);
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
            [Trait("Category", "Unit")]
            public void Should_ThrowExceptionInvalidEntityName_When_UsingAnEntityNameThatCanCauseValidationTimeouts()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithCustomEntityNameValidationTimeout(1000000);
                var factory = new HiLoGeneratorFactory(mockConfig);
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

        #region Utils

        public static IConfiguration CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20()
        {
            var appSettings = @"{
                ""NHiLo"":{
                    ""DefaultMaxLo"" : ""100""
                },
                ""ConnectionStrings"":{
                    ""NHiLo"":{
                        ""ConnectionString"":""test"",
                        ""ProviderName"":""NHiLo.InMemory""
                    }
                }
            }";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            return builder.Build();
        }

        public static IConfiguration CreateHiloConfigurationWithNoConfigurationForNHiLoAndOnlyOneConnectionStringWithNoProviderName()
        {
            var appSettings = @"{
                ""ConnectionStrings"":{
                    ""NHiLo"": ""test""
                }
            }";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            return builder.Build();
        }

        public static IConfiguration CreateHiloConfigurationWithCustomEntityNameValidationTimeout(int entityNameValidationTimeout)
        {
            var appSettings = $@"{{
                    ""NHiLo"":{{
                        ""EntityNameValidationTimeout"": ""{entityNameValidationTimeout}""
                    }},
                    ""ConnectionStrings"":{{
                        ""NHiLo"":{{
                            ""ConnectionString"":""anything"",
                            ""ProviderName"":""Microsoft.Data.SqlClient""
                        }}
                    }}
                }}";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            return builder.Build();
        }

        #endregion
    }
}
