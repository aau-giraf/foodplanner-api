using Test.Builder;

namespace Test.BuilderTest;

public class ClassroomBuilderTest
{
    [Fact]
    public void ClassroomBuilder_ReturnsClassroom_WithDefaultValue()
    {
        //Arrange
        var builder = new ClassroomBuilder();
        
        //Act
        var classroom = builder.Build();
        
        //Assert
        Assert.NotNull(classroom);
    }

    [Theory]
    [InlineData(2,"Testroom")]
    [InlineData(2,"Cafeteria")]

    public void ChildBuilder_ReturnsChild_WithExpectedValues(int classId, string className)
    {
        //Arrange 
        var builder = new ClassroomBuilder().WithClassId(classId).WithClassName(className);
        
        //Act
        var child = builder.Build();
        
        //Assert
        Assert.Equal(classId, child.ClassId);
        Assert.Equal(className, child.ClassName);
    }
}