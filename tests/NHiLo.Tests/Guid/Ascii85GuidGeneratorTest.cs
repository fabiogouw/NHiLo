using Xunit;
using NHiLo.Guid;

namespace NHiLo.Tests.Guid
{
    public class Ascii85GuidGeneratorTest
    {
        public class GetKey : BaseTestGuidGenerator.GetKey
        {
            [Fact]
            public void ShouldGetANonNullAscii85Guid()
            {
                // Arrange
                var generator = new Ascii85GuidGenerator();
                // Act & Assert
                ShouldGetANonNullGuid(generator);
            }

            [Fact]
            public void ShouldGetDifferentAscii85Keys()
            {
                // Arrange
                var generator = new Ascii85GuidGenerator();
                // Act & Assert
                ShouldGetDifferentKeys(generator);
            }

            [Fact]
            public void ShouldGet20LengthGuid()
            {
                // Arrange
                var generator = new Ascii85GuidGenerator();
                // Act
                var key = generator.GetKey();
                // Assert
                Assert.Equal(20, key.Length);
            }

        }
    }
}
