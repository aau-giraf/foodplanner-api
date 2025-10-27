using FoodplannerModels.Account;

namespace Test.Builder;

public class ChildBuilder
{
    private int _childId = 1;
    private string _firstName = "Test";
    private string _lastName = "Child";
    private int _classId = 1;

    public ChildBuilder WithChildId(int childId)
    {
        _childId = childId;
        return this;
    }

    public ChildBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public ChildBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public ChildBuilder WithClassId(int classId)
    {
        _classId = classId;
        return this;
    }

    public Children Build()
    {
        return new Children
        {
            ChildId = _childId,
            FirstName = _firstName,
            LastName = _lastName,
            classId = _classId
        };
    }
}
