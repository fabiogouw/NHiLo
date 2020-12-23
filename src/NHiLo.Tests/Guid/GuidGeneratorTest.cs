using NHiLo.Guid;
using Xunit;

namespace NHiLo.Tests.Guid
{
    public class GuidGeneratorTest
    {
        public class GetKey : BaseTestGuidGenerator.GetKey
        {
            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetANonNullGuidv4()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act & Assert
                ShouldGetANonNullGuid(generator);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ShouldGetDifferentKeysv4()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act & Assert
                ShouldGetDifferentKeys(generator);
            }
        }
    }
}
