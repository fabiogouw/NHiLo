using NHiLo.Guid;
using Xunit;
using FluentAssertions;
using System.Text;

namespace NHiLo.Tests.Guid
{
    public class Ascii85Test
    {
        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("Any colour you like")]
        public void ShouldAEncondedValueBeDecodedToTheOriginal(string value)
        {
            // Arrange
            var sut = new Ascii85();
            // Act
            string encoded = sut.Encode(Encoding.ASCII.GetBytes(value));
            byte[] decoded = sut.Decode($"<~{encoded}~>");
            string result = Encoding.ASCII.GetString(decoded);
            // Assert
            result.Should().Be(value);
        }

        public class Decode
        {
            [Theory]
            [Trait("Category", "Unit")]
            [InlineData("value without the prefix and sufix")]
            [InlineData("<~just prefix")]
            [InlineData("just sufix~>")]
            public void ShouldThrowErrorWhenDecodeAValueWithoutPrefixOrSufix(string value)
            {
                // Arrange
                var sut = new Ascii85();
                sut.EnforceMarks = true;
                // Act & Assert
                Assert.Throws<NHiLoException>(() => sut.Decode(value));
            }
        }
    }
}
