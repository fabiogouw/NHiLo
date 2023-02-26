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
        public void Should_AEncondedValueBeDecodedToTheOriginal_When_TheValueIsEncodedAndDecodedSequentially(string value)
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
            // <~6#Lrj@rH1%F`JUMDfm1=Bkq8~> is an example of valid encoded string
            [Theory]
            [Trait("Category", "Unit")]
            [InlineData("value-without-the-prefix-and-sufix", "ASCII85 encoded data should begin with '<~' and end with '~>'")]  // without prefix and sufix
            [InlineData("<~without-sufix", "ASCII85 encoded data should begin with '<~' and end with '~>'")]     // without sufix
            [InlineData("without-prefix~>", "ASCII85 encoded data should begin with '<~' and end with '~>'")]    // without prefix
            [InlineData("<~z~>", "Bad character 'z' found. ASCII85 only allows characters '!' to 'u'.")]   // char z
            [InlineData("<~invalid~>", "Bad character 'v' found. ASCII85 only allows characters '!' to 'u'.")]   // char v
            [InlineData("<~ ~>", "Bad character ' ' found. ASCII85 only allows characters '!' to 'u'.")]   // spaces are invalid
            [InlineData("<~w#Lrj@rH1%F`JUMDfm1=Bkq8~>", "Bad character 'w' found. ASCII85 only allows characters '!' to 'u'.")]   // the first w is an invalid char
            
            public void Should_ThrowError_When_DecodingAStringWithoutInvalidCondition(string value, string message)
            {
                // Arrange
                var sut = new Ascii85();
                sut.EnforceMarks = true;
                try
                {
                    // Act
                    sut.Decode(value);
                }
                catch (NHiLoException ex)
                {
                    // Assert
                    ex.Data["reason"].Should().Be(message);
                }
            }
        }
    }
}
