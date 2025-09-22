using FoodplannerModels.Account;

namespace Test.Builder;

public class ChildBuilder : Children
{
    public ChildBuilder()
    {
        this.FirstName = "testFirstName";
        this.LastName = "testLastName";
    }

    public ChildBuilder WithId(int childId)
    {
        this.ChildId = childId;
        return this;
    }
    
    public ChildBuilder WithFirstName(string firstName)
    {
        this.FirstName = firstName;
        return this;
    }
    
    public ChildBuilder WithLastName(string lastName)
    {
        this.LastName = lastName;
        return this;
    }
    
    public ChildBuilder WithParentId(int parentId)
    {
        this.parentId = parentId;
        return this;
    }

    public ChildBuilder WithClassId(int classId)
    {
        this.classId = classId;
        return this;
    }

    public Children Build()
    {
        return new Children()
        {
            ChildId = this.ChildId,
            FirstName = this.FirstName,
            LastName = this.LastName,
            parentId = this.parentId,
            classId = this.classId,
        };
    }
}