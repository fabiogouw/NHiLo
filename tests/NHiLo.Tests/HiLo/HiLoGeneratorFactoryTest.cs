using Microsoft.Extensions.Configuration;
using Moq;
using NHiLo.HiLo;
using NHiLo.HiLo.Config;
using NHiLo.HiLo.Repository;
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
            public void ShouldReturnAnInstanceOfHiLoGenerator()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                var generator = factory.GetKeyGenerator("dummy");
                // Assert
                Assert.IsAssignableFrom<HiLoGenerator>(generator);
            }
            
            [Fact]
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
            public void ShouldRaiseExceptionIfEntityNameStartsWithANumber()
            {
                ShouldRaiseExceptionIfEntityNameIsInvalid("123name");
            }

            [Fact]
            public void ShouldRaiseExceptionIfEntityNameContainsSpaces()
            {
                ShouldRaiseExceptionIfEntityNameIsInvalid("n ame");
            }

            [Fact]
            public void ShouldRaiseExceptionIfEntityNameContainsSingleQuotes()
            {
                ShouldRaiseExceptionIfEntityNameIsInvalid("name'");
            }

            [Fact]
            public void ShouldRaiseExceptionIfEntityNameHasMoreThan100Chars()
            {
                ShouldRaiseExceptionIfEntityNameIsInvalid("namesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnamesnames1");
            }

            private void ShouldRaiseExceptionIfEntityNameIsInvalid(string entityName)
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                var factory = new HiLoGeneratorFactory(mockConfig);
                // Act
                try
                {
                    factory.GetKeyGenerator(entityName);
                    throw new XunitException();
                }
                catch (NHiloException ex)
                {
                    // Assert
                    Assert.Equal(ErrorCodes.InvalidEntityName, ex.ErrorCode);
                }
            }
        }

        #region Utils

        public static IConfiguration CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20()
        {
            var appSettings = @"{
                ""NHilo"":{
                    ""DefaultMaxLo"" : ""100""
                },
                ""ConnectionStrings"":{
                    ""NHiLo"":{
                        ""ConnectionString"":""test"",
                        ""ProviderName"":""NHilo.InMemory""
                    }
                }
            }";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            return builder.Build();
        }

        #endregion
    }
}
