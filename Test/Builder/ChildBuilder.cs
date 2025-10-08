using FoodplannerModels.Account;

namespace Test.Builder;

public class ChildBuilder
{
    private int _childId;
    private string _firstName = "testFirstName";
    private string _lastName = "testLastName";
    private int _parentId;
    private int _classId;

    public ChildBuilder WithId(int id)
    {
        _childId = id;
        return this;
    }

    public ChildBuilder WithFirstName(string value)
    {
        _firstName = value;
        return this;
    }

    public ChildBuilder WithLastName(string value)
    {
        _lastName = value;
        return this;
    }

    public ChildBuilder WithParentId(int id)
    {
        _parentId = id;
        return this;
    }

    public ChildBuilder WithClassId(int id)
    {
        _classId = id;
        return this;
    }

    public Children Build()
    {
        return new Children()
        {
            ChildId = _childId,
            FirstName = _firstName,
            LastName = _lastName,
            parentId = _parentId,
            classId = _classId
        };
    }
}