using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHiLo.HiLo;
using Moq;

namespace NHiLo.Tests.HiLo
{
    public class HiLoGeneratorTest
    {
        [TestClass]
        public class ctor
        {
            [TestMethod]
            public void ShouldFailCreatingInstanceWithNullAsRepository()
            {
                // Arrange
                IHiLoRepository repository = null;
                try
                {
                    // Act
                    var generator = new HiLoGenerator(repository, 0);
                    // Assert
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                }
            }

            [TestMethod]
            public void ShouldFailCreatingInstanceWith0AsMaxLo()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                try
                {
                    // Act
                    var generator = new HiLoGenerator(mock.Object, 0);
                    // Assert
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                }
            }
        }

        [TestClass]
        public class GetKey
        {
            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
            public void ShouldGetTheFirstKeyFromTheHiValue()
            {
                // Arrange
                var mock = new Mock<IHiLoRepository>();
                mock.Setup(m => m.GetNextHi()).Returns(1);
                var generator = new HiLoGenerator(mock.Object, 10);
                // Act
                long key = generator.GetKey();
                // Assert
                Assert.AreEqual(10, key);
            }

            [TestMethod]
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
                Assert.AreEqual(11, key);
            }

            [TestMethod]
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
                Assert.AreEqual(20, key);
            }

            [TestMethod]
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
                    Assert.IsFalse(keys.Contains(key));
                    keys.Add(key);
                }
            }
        }
    }
}
