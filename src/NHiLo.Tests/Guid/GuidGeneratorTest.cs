using Xunit;
using NHiLo.Guid;

namespace NHiLo.Tests.Guid
{
    public class GuidGeneratorTest
    {
        public class GetKey : BaseTestGuidGenerator.GetKey
        {
            [Fact]
            public void ShouldGetANonNullGuidv4()
            {
                // Arrange
                var generator = new GuidGenerator();
                // Act & Assert
                ShouldGetANonNullGuid(generator);
            }

            [Fact]
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
