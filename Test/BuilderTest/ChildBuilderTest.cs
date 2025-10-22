using Test.Builder;
using FoodplannerModels.Account;
using Xunit;

namespace Test.BuilderTest;

public class ChildBuilderTest
{
    [Fact]
    public void Build_ShouldCreateChildWithCorrectProperties()
    {
        // Arrange
        var builder = new ChildBuilder()
            .WithChildId(1)
            .WithFirstName("John")
            .WithLastName("Doe")
            .WithClassId(2);

        // Act
        var child = builder.Build();

        // Assert
        Assert.Equal(1, child.ChildId);
        Assert.Equal("John", child.FirstName);
        Assert.Equal("Doe", child.LastName);
        Assert.Equal(2, child.classId);
    }

    [Fact]
    public void Build_ShouldCreateChildWithDefaultValues()
    {
        // Arrange & Act
        var child = new ChildBuilder().Build();

        // Assert
        Assert.Equal(1, child.ChildId);
        Assert.Equal("Test", child.FirstName);
        Assert.Equal("Child", child.LastName);
        Assert.Equal(1, child.classId);
    }
}
