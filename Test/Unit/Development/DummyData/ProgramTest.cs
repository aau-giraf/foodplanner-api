using Xunit;

namespace Test.Development.DummyData;

public class ProgramTest
{
    [Theory]
    [InlineData("all")]
    [InlineData("core")]
    [InlineData("foodplanner")]
    [Trait("Category", "Unit")]
    public void TryParseTarget_ValidTarget_ReturnsTrue(string input)
    {
        // Act
        var result = global::DummyData.Program.TryParseTarget(
            [input], out var target);

        // Assert
        Assert.True(result);
        Assert.Equal(input, target);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParseTarget_NoArguments_DefaultsToAll()
    {
        // Act
        var result = global::DummyData.Program.TryParseTarget(
            [], out var target);

        // Assert
        Assert.True(result);
        Assert.Equal("all", target);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("database")]
    [InlineData("test")]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Category", "Unit")]
    public void TryParseTarget_InvalidTarget_ReturnsFalse(string input)
    {
        // Act
        var result = global::DummyData.Program.TryParseTarget(
            [input], out _);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("ALL", "all")]
    [InlineData("CORE", "core")]
    [InlineData("FOODPLANNER", "foodplanner")]
    [InlineData("FoodPlanner", "foodplanner")]
    [Trait("Category", "Unit")]
    public void TryParseTarget_MixedCase_NormalizesTarget(
        string input,
        string expected)
    {
        // Act
        var result = global::DummyData.Program.TryParseTarget(
            [input], out var target);

        // Assert
        Assert.True(result);
        Assert.Equal(expected, target);
    }

    [Theory]
    [InlineData("core", "invalid")]
    [InlineData("foodplanner", "extra")]
    [InlineData("all", "unexpected")]
    [Trait("Category", "Unit")]
    public void TryParseTarget_MultipleArguments_UsesFirstArgument(
        string first,
        string second)
    {
        // Act
        var result = global::DummyData.Program.TryParseTarget(
            [first, second], out var target);

        // Assert
        Assert.True(result);
        Assert.Equal(first, target);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FindApiDirectory_ValidPath_ReturnsApiDirectory()
    {
        // Arrange
        var root = Directory.CreateTempSubdirectory("dummydata-test-");

        var apiPath = Path.Combine(root.FullName, "FoodplannerApi");
        var startPath = Path.Combine(
            root.FullName, "Development", "DummyData"
        );

        try
        {
            Directory.CreateDirectory(apiPath);
            Directory.CreateDirectory(startPath);

            // Act
            var result = global::DummyData.Program.FindApiDirectory(
                startPath
            );

            // Assert
            Assert.Equal(apiPath, result);
        }
        finally
        {
            // Clean up temporary directories
            root.Delete(true);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FindApiDirectory_MissingApiDirectory_ThrowsException()
    {
        // Arrange
        var root = Directory.CreateTempSubdirectory("dummydata-test-");

        try
        {
            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() =>
                global::DummyData.Program.FindApiDirectory(
                    root.FullName
                )
            );
        }
        finally
        {
            // Clean up temporary directories
            root.Delete(true);
        }
    }

    [Theory]
    [InlineData("Development", true)]
    [InlineData("development", true)]
    [InlineData("DEVELOPMENT", true)]
    [InlineData("Production", false)]
    [InlineData("Staging", false)]
    [InlineData("Testing", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [Trait("Category", "Unit")]
    public void IsAllowedEnvironment_ValidatesEnvironment(
        string? environment,
        bool expected)
    {
        // Act
        var result = global::DummyData.Program.IsAllowedEnvironment(
            environment
        );

        // Assert
        Assert.Equal(expected, result);
    }
}