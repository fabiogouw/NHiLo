using NHiLo.Guid;
using Xunit;
using FluentAssertions;
using System;

namespace NHiLo.Tests.Guid
{
    public class GuidGeneratorFactoryTest
    {
        public class GetKeyGenerator
        {
            [Theory]
            [Trait("Category", "Unit")]
            [InlineData(typeof(GuidGeneratorFactory))]
            [InlineData(typeof(Ascii85GuidGeneratorFactory))]
            public void ShouldGetGuidGenerator(Type type)
            {
                // Arrange
                var sut = Activator.CreateInstance(type) as IKeyGeneratorFactory<string>;
                // Act
                var result = sut.GetKeyGenerator("any");
                // Assert
                result.Should().BeAssignableTo<IKeyGenerator<string>>();
            }
        }
    }
}
