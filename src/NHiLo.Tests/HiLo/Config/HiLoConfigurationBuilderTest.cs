using NHiLo.HiLo.Config;
using Xunit;
using System;
using System.Configuration;
using Moq;
using NHiLo.Common.Config;
using NHiLo.Common;
using NHiLo.Tests.TestDoubles.Config;
using Xunit.Sdk;

namespace NHiLo.Tests.HiLo.Config
{
    public class HiLoConfigurationBuilderTest
    {
        public class Build
        {
            [Fact]
            public void ShouldThrowAnExceptionWhenTheresNoConnectionStringAvailableAtConfiguration()
            {
                // Arrange
                var mock = new Mock<IConfigurationManager>();
                var target = new HiLoConfigurationBuilder(mock.Object);
                try
                {
                    // Act
                    var actual = target.Build();
                    throw new XunitException();
                }
                catch (NHiloException ex)
                {
                    // Assert
                    Assert.Equal(ErrorCodes.NoConnectionStringAvailable, ex.ErrorCode);
                }
            }

            [Fact]
            public void ShouldReturnTheLastConnectionStringInConfigFileAsTheConnectionForNHiLosUseWhenTheConnectionWasntSpecified()
            {
                // Arrange
                var mock = new Mock<IConfigurationManager>();
                mock = ConfigureConfigurationManagerMockWith2ConnectionStringsSettings(mock);
                var target = new HiLoConfigurationBuilder(mock.Object);
                // Act
                var actual = target.Build();
                // Assert
                Assert.Equal("fake connection string", actual.ConnectionString);
                Assert.Equal("fake provider", actual.ProviderName);
            }
            /*
            [Fact]
            public void ShouldReturnTheConnectionStringInConfigFileSpecifiedByKeyGeneratorConfigurationSection()
            {
                // Arrange
                var hiloConfigurationStub = new HiloConfigurationStub() { ConnectionStringId = "first" };
                var keyGeneratorConfigurationStub = new KeyGeneratorConfigurationStub() { HiloKeyGeneratorFunction = () => hiloConfigurationStub };
                var mock = new Mock<IConfigurationManager>();
                mock.Setup<IKeyGeneratorConfiguration>(m => m.GetSection<IKeyGeneratorConfiguration>(It.Is<string>(p => p == "nhilo"))).Returns(keyGeneratorConfigurationStub);
                mock = ConfigureConfigurationManagerMockWith2ConnectionStringsSettings(mock);
                var target = new HiLoConfigurationBuilder(mock.Object);
                // Act
                var actual = target.Build();
                // Assert
                Assert.Equal("first connection string", actual.ConnectionString);
                Assert.Equal("first provider", actual.ProviderName);
            }

            [Fact]
            public void ShouldThrowAnExceptionWhenTheConnectionStringAvailableAtConfigurationDoesNotMatchWithTheOnesInConfigFile()
            {
                // Arrange
                var hiloConfigurationStub = new HiloConfigurationStub() { ConnectionStringId = "unavailable" };
                var keyGeneratorConfigurationStub = new KeyGeneratorConfigurationStub() { HiloKeyGeneratorFunction = () => hiloConfigurationStub };
                var mock = new Mock<IConfigurationManager>();
                mock.Setup<IKeyGeneratorConfiguration>(m => m.GetSection<IKeyGeneratorConfiguration>(It.Is<string>(p => p == "nhilo"))).Returns(keyGeneratorConfigurationStub);
                mock = ConfigureConfigurationManagerMockWith2ConnectionStringsSettings(mock);
                var target = new HiLoConfigurationBuilder(mock.Object);
                try
                {
                    // Act
                    var actual = target.Build();
                    throw new XunitException();
                }
                catch (NHiloException ex)
                {
                    // Assert
                    Assert.Equal(ErrorCodes.NoConnectionStringAvailable, ex.ErrorCode);
                }
            }
            */
        }

        #region Utils

        private static Mock<IConfigurationManager> ConfigureConfigurationManagerMockWith2ConnectionStringsSettings(Mock<IConfigurationManager> mock)
        {
            var connectionStringSection = new ConnectionStringsSection();
            connectionStringSection.ConnectionStrings.Add(new ConnectionStringSettings() { Name = "first", ConnectionString = "first connection string", ProviderName = "first provider" });
            connectionStringSection.ConnectionStrings.Add(new ConnectionStringSettings() { Name = "fake", ConnectionString = "fake connection string", ProviderName = "fake provider" });
            mock.Setup<ConnectionStringsSection>(m => m.GetConnectionStringsSection()).Returns(connectionStringSection);
            return mock;
        }

        #endregion
    }
}
