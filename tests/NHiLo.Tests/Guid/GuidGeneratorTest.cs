using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHiLo.Guid;

namespace NHiLo.Tests.Guid
{
    public class GuidGeneratorTest
    {
        [TestClass]
        public class GetKey : BaseTestGuidGenerator.GetKey
        {
            [TestMethod]
            public void ShouldGetANonNullGuid()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act & Assert
                ShouldGetANonNullGuid(generator);
            }

            [TestMethod]
            public void ShouldGetDifferentKeys()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act & Assert
                ShouldGetDifferentKeys(generator);
            }
        }
    }
}
