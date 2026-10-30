using FoodplannerModels.Account;

namespace Test.Builder;

public class ClassroomBuilder 
{
    private int _classId;
    private string _className = "testClassName";

  

    public ClassroomBuilder WithClassId(int classId)
    {
        _classId = classId;
        return this;
    }
    
    public ClassroomBuilder WithClassName(string className)
    {
        _className = className;
        return this;
    }

    public Classroom Build()
    {
        return new Classroom
        {
            ClassId = _classId,
            ClassName = _className,
        };
    }
}