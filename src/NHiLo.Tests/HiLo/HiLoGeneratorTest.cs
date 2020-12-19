using Xunit;
using Moq;
using NHiLo.HiLo;
using System;
using System.Collections.Generic;
using Xunit.Sdk;

namespace NHiLo.Tests.HiLo
{
    public class HiLoGeneratorTest
    {
        public class ctor
        {
            [Fact]
            public void ShouldFailCreatingInstanceWithNullAsRepository()
            {
                // Arrange
                IHiLoRepository repository = null;
                try
                {
                    // Act
                    var generator = new HiLoGenerator(repository, 0);
                    // Assert
                    throw new XunitException();
                }
                catch (ArgumentException)
                {
                }
            }

            [Fact]
            public void ShouldFailCreatingInstanceWith0AsMaxLo()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                try
                {
                    // Act
                    var generator = new HiLoGenerator(mock.Object, 0);
                    // Assert
                    throw new XunitException();
                }
                catch (ArgumentException)
                {
                }
            }
        }

        public class GetKey
        {
            [Fact]
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
