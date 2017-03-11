using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHiLo.Guid;

namespace NHiLo.Tests.Guid
{
    public class Ascii85GuidGeneratorTest
    {
        [TestClass]
        public class GetKey : BaseTestGuidGenerator.GetKey
        {
            [TestMethod]
            public void ShouldGetANonNullGuid()
            {
                // Arrange
                var generator = new Ascii85GuidGenerator();
                // Act & Assert
                ShouldGetANonNullGuid(generator);
            }

            [TestMethod]
            public void ShouldGetDifferentKeys()
            {
                // Arrange
                var generator = new Ascii85GuidGenerator();
                // Act & Assert
                ShouldGetDifferentKeys(generator);
            }

            [TestMethod]
            public void ShouldGet20LengthGuid()
            {
                // Arrange
                var generator = new Ascii85GuidGenerator();
                // Act
                var key = generator.GetKey();
                // Assert
                Assert.AreEqual(20, key.Length);
            }

        }
    }
}
