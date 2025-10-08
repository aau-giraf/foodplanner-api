using Test.Builder;

namespace Test.BuilderTest;

public class ChildBuilderTest
{
    [Fact]
    public void ChildBuilder_ReturnsChild_ThatIsNotNull()
    {
        //Arrange 
        var builder = new ChildBuilder();
        
        //Act
        var child = builder.Build();
        
        //Assert
        Assert.NotNull(child);
    }

    [Theory]
    [InlineData(2, "John", "Doe", 2, 3)]
    [InlineData(1, "Alice", "Smith", 5, 7)]
    public void ChildBuilder_ReturnsChild_WithExpectedValues(int id, string first, string last, int parentId, int classId)
    {
        //Arrange
        var Builder = new ChildBuilder()
            .WithId(id)
            .WithFirstName(first)
            .WithLastName(last)
            .WithParentId(parentId)
            .WithClassId(classId);
        
        //Act
        var child = Builder.Build();
        
        //Assert
        Assert.Equal(id, child.ChildId);
        Assert.Equal(first, child.FirstName);
        Assert.Equal(last, child.LastName);
        Assert.Equal(parentId, child.parentId);
        Assert.Equal(classId, child.classId);
    }
}
