using FoodplannerModels.Account;

namespace Test.Builder;

public class ClassroomBuilder : Classroom
{
    public ClassroomBuilder()
    {
        this.ClassName = "testClassName";
    }
    public ClassroomBuilder WithClassName(string className)
    {
        this.ClassName = className;
        return this;
    }
    
    public ClassroomBuilder WithClassId(int classId)
    {
        this.ClassId = classId;
        return this;
    }
    
    public Classroom Build()
    {
        return new Classroom
        {
            ClassId = this.ClassId,
            ClassName = this.ClassName,
        };
    }
}