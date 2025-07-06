using FluentAssertions;
using Moq;
using NHiLo.HiLo;
using NHiLo.HiLo.Repository;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace NHiLo.Tests.HiLo
{
    [Trait("Category", "Unit")]
    public class HiLoGeneratorTest
    {
        public class Ctor
        {
            [Fact]
            [Trait("Category", "Unit")]
            public void Should_Fail__When__Creating_Instance_With_Null_As_Repository()
            {
                // Arrange
                IHiLoRepository repository = null;
                // Act
                Action act = () => new HiLoGenerator(repository, 0);
                // Assert
                act.Should().Throw<ArgumentException>();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldFailCreatingInstanceWith0AsMaxLo()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                // Act
                Action act = () => new HiLoGenerator(mock.Object, 0);
                // Assert
                act.Should().Throw<ArgumentException>();
            }
        }

        public class GetKey
        {
            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetTheHiValueFromRepository()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(1);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                generator.GetKey();
                // Assert
                mock.Verify(m => m.GetNextHi(), Times.Once());
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetTheHiValueFromRepositoryOnceFor10KeyGenerationsWith10AsMaxLo()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(1);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                for (int i = 1; i <= 10; i++)
                    generator.GetKey();
                // Assert
                mock.Verify(m => m.GetNextHi(), Times.Once());
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetTheHiValueFromRepositoryTwiceFor11KeyGenerationsWith10AsMaxLo()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(1);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                for (int i = 1; i <= 11; i++)
                    generator.GetKey();
                // Assert
                mock.Verify(m => m.GetNextHi(), Times.Exactly(2));
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetTheFirstKeyFromTheHiValue()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(1);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                long key = generator.GetKey();
                // Assert
                Assert.Equal(10, key);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetTheSecondKeyFromTheHiValue()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(1);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                generator.GetKey();
                long key = generator.GetKey();
                // Assert
                Assert.Equal(11, key);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetThe11thKeyFromTheHiValue()
            {
                // Arrange
                int hi = 1;
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(() => hi++);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                for (int i = 1; i <= 10; i++)
                    generator.GetKey();
                long key = generator.GetKey();
                // Assert
                Assert.Equal(20, key);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetDifferentKeysForEachCall()
            {
                // Arrange
                List<long> keys = new List<long>();
                int hi = 1;
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(() => hi++);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                for (int i = 1; i <= 30; i++)
                {
                    long key = generator.GetKey();
                    // Assert
                    Assert.DoesNotContain(key, keys);
                    keys.Add(key);
                }
            }
        }
    }
}
