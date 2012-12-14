using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHiLo.HiLo;
using NHiLo.HiLo.Repository;
using Moq;
using NHiLo.HiLo.Config;

namespace NHiLo.Tests.HiLo
{
    public class HiLoGeneratorFactoryTest
    {
        [TestClass]
        public class GetKeyGenerator
        {
            [TestMethod]
            public void ShouldReturnAnInstanceOfHiLoGenerator()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                mockConfig.SetupGet(p => p.ConnectionStringId).Returns("");
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator = factory.GetKeyGenerator("dummy");
                // Assert
                Assert.IsInstanceOfType(generator, typeof(HiLoGenerator));
            }

            [TestMethod]
            public void ShouldReturnTheSameInstanceForTheSameEntity()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                mockConfig.SetupGet(p => p.ConnectionStringId).Returns("");
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator1 = factory.GetKeyGenerator("dummy");
                var generator2 = factory.GetKeyGenerator("dummy");
                // Assert
                Assert.AreSame(generator1, generator2);
            }

            [TestMethod]
            public void ShouldReturnTheSameInstanceForTheSameEntityAndForDifferentFactories()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                mockConfig.SetupGet(p => p.ConnectionStringId).Returns("");
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory1 = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                var factory2 = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator1 = factory1.GetKeyGenerator("dummy");
                var generator2 = factory2.GetKeyGenerator("dummy");
                // Assert
                Assert.AreSame(generator1, generator2);
            }

            [TestMethod]
            public void ShouldReturnDifferentInstancesForDifferentEntities()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                mockConfig.SetupGet(p => p.ConnectionStringId).Returns("");
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator1 = factory.GetKeyGenerator("dummy1");
                var generator2 = factory.GetKeyGenerator("dummy2");
                // Assert
                Assert.AreNotSame(generator1, generator2);
            }

            [TestMethod]
            public void ShouldReturnDifferentInstancesForDifferentEntitiesAndForDifferentFactories()
            {
                // Arrange
                var mockConfig = CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20();
                mockConfig.SetupGet(p => p.ConnectionStringId).Returns("");
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory1 = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                var factory2 = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator1 = factory1.GetKeyGenerator("dummy1");
                var generator2 = factory2.GetKeyGenerator("dummy2");
                // Assert
                Assert.AreNotSame(generator1, generator2);
            }

            [TestMethod]
            public void ShouldCreateGeneratorGettingTheDefaultMaxLoFromConfig()
            {
                // Arrange
                int chamadasEntityDummy = 0;
                var mockConfig = new Mock<IHiLoConfiguration>();
                mockConfig.SetupGet(p => p.DefaultMaxLo).Returns(100);
                mockConfig.Setup(m => m.GetEntityConfig(It.Is<string>(p => p == "dummy4"))).Returns(null as IEntityConfiguration).Callback(() => chamadasEntityDummy++);
                var mockRepFac = new Mock<IHiLoRepositoryFactory>() { DefaultValue = DefaultValue.Mock };
                var factory = new HiLoGeneratorFactory(mockRepFac.Object, mockConfig.Object);
                // Act
                var generator = factory.GetKeyGenerator("dummy4");
                // Assert
                Assert.AreEqual(1, chamadasEntityDummy);    // HACK: there's a problem with Moq where I can't verify if p => p["dummy"] was called
                mockConfig.VerifyGet(p => p.DefaultMaxLo, Times.Once());
            }

            [TestMethod]
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
        }

        #region Utils

        public static Mock<IHiLoConfiguration> CreateHiloConfigurationWithDefaultMaxLo100AndFirstEntityWithMaxLo20()
        {
            var mockConfig = new Mock<IHiLoConfiguration>();
            mockConfig.SetupGet(p => p.DefaultMaxLo).Returns(100);
            var mockEntity1 = new Mock<IEntityConfiguration>();
            mockEntity1.SetupGet(p => p.MaxLo).Returns(20);
            mockConfig.Setup(m => m.GetEntityConfig(It.IsAny<string>())).Returns(mockEntity1.Object);
            return mockConfig;
        }

        #endregion
    }
}
