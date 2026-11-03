using Xunit;

namespace Test.Development.DummyData;

public class ProgramTest
{
    [Theory]
    [InlineData("all")]
    [InlineData("core")]
    [InlineData("foodplanner")]
    public void TryParseTarget_ValidTarget_ReturnsTrue(string input)
    {
        // Act
        var result = global::DummyData.Program.TryParseTarget(
            [input], out var target);

        // Assert
        Assert.True(result);
        Assert.Equal(input, target);
    }
}