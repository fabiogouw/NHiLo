using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHiLo.Guid;

namespace NHiLo.Tests.Guid
{
    public class GuidGeneratorTest
    {
        [TestClass]
        public class GetKey
        {
            [TestMethod]
            public void ShouldGetANonNullGuid()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act
                string key = generator.GetKey();
                // Assert
                Assert.AreNotEqual(System.Guid.Empty.ToString(), key);
            }

            [TestMethod]
            public void ShouldGetDifferentKeys()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act
                string key1 = generator.GetKey();
                string key2 = generator.GetKey();
                // Assert
                Assert.AreNotEqual(key1, key2);
            }
        }
    }
}
