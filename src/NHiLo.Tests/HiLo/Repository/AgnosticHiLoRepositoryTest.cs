using Moq;
using NHiLo.HiLo.Config;
using NHiLo.HiLo.Repository;
using NHiLo.Tests.TestDoubles;
using System.Data;
using System.Data.Common;
using Xunit;

namespace NHiLo.Tests.HiLo.Repository
{
    public class AgnosticHiLoRepositoryTest
    {
        private class TestableHiLoRepository : AgnosticHiLoRepository
        {
            public TestableHiLoRepository(IHiLoConfiguration config, DbProviderFactory provider)
                : base(config, provider)
            {
            }

            internal int CallsToGetNextHiFromDatabase { get; private set; }
            internal int CallsToCreateRepositoryStructure { get; private set; }
            internal int CallsToInitializeRepositoryForEntity { get; private set; }

            protected override long GetNextHiFromDatabase(IDbCommand cmd)
            {
                CallsToGetNextHiFromDatabase++;
                return 0;
            }

            protected override void CreateRepositoryStructure(IDbCommand cmd)
            {
                CallsToCreateRepositoryStructure++;
            }

            protected override void InitializeRepositoryForEntity(IDbCommand cmd)
            {
                CallsToInitializeRepositoryForEntity++;
            }
        }

        public class PrepareRepository
        {
            private Mock<IHiLoConfiguration> SetupConfiguration(Mock<IHiLoConfiguration> mock, bool createHiLoStructureIfNotExists)
            {
                mock.SetupGet(p => p.CreateHiLoStructureIfNotExists).Returns(createHiLoStructureIfNotExists);
                mock.SetupGet(p => p.ConnectionString).Returns("Valid Config");
                return mock;
            }

            private Mock<DbProviderFactory> SetupDbProvider(Mock<DbProviderFactory> mock)
            {
                var mockConnection = new DbConnectionMock();
                mock.Setup(m => m.CreateConnection()).Returns(mockConnection);
                return mock;
            }

            private TestableHiLoRepository CreateSystemUnderTest(Mock<IHiLoConfiguration> mockConfig, Mock<DbProviderFactory> mockDbProviderFactory)
            {
                var target = new TestableHiLoRepository(mockConfig.Object, mockDbProviderFactory.Object);
                return target;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldInvokeTheInitializeRepositoryMethodWithoutCreatingRepositoryStructure()
            {
                // Arrange
                var mockConfig = new Mock<IHiLoConfiguration>();
                mockConfig = SetupConfiguration(mockConfig, false);
                var mockDbProviderFactory = new Mock<DbProviderFactory>();
                mockDbProviderFactory = SetupDbProvider(mockDbProviderFactory);
                var target = CreateSystemUnderTest(mockConfig, mockDbProviderFactory);
                // Act
                target.PrepareRepository("");
                // Assert
                Assert.Equal(1, target.CallsToInitializeRepositoryForEntity);
                Assert.Equal(0, target.CallsToCreateRepositoryStructure);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldInvokeTheInitializeRepositoryMethodCreatingRepositoryStructure()
            {
                // Arrange
                var mockConfig = new Mock<IHiLoConfiguration>();
                mockConfig = SetupConfiguration(mockConfig, true);
                var mockDbProviderFactory = new Mock<DbProviderFactory>();
                mockDbProviderFactory = SetupDbProvider(mockDbProviderFactory);
                var target = CreateSystemUnderTest(mockConfig, mockDbProviderFactory);
                // Act
                target.PrepareRepository("");
                // Assert
                Assert.Equal(1, target.CallsToInitializeRepositoryForEntity);
                Assert.Equal(1, target.CallsToCreateRepositoryStructure);
            }

        }
    }
}
