using FluentAssertions;
using Xunit;

namespace DressShop.UnitTests;

public class SmokeTests
{
    [Fact]
    public void UnitTestProject_IsWiredCorrectly()
    {
        var result = 2 + 2;

        result.Should().Be(4);
    }
}
