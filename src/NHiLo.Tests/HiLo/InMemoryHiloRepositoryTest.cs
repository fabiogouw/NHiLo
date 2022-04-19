using NHiLo.Guid;
using Xunit;
using FluentAssertions;
using NHiLo.HiLo.Repository;

namespace NHiLo.Tests.HiLo
{
    public class InMemoryHiloRepositoryTest
    {
        public class GetNextHi
        {
            [Fact]
            public void ShouldReturnTheNextHiValue()
            {
                var sut = new InMemoryHiloRepository();
                sut.PrepareRepository("myEntity");
                sut.GetNextHi().Should().Be(1);
            }
        }
    }
}
