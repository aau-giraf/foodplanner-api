using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class MealBuilder
{
    private int _id = 99999;
    private string _name = "testMeal";
    private string _date = "11/11/2050";
    private int _userId;
    private int? _foodImageId;
    private IEnumerable<PackedDTO> _packedIngredient = [];

    public MealBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public MealBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public MealBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public MealBuilder WithFoodImageId(int? foodImageId)
    {
        _foodImageId = foodImageId;
        return this;
    }

    public MealBuilder WithDate(string date)
    {
        _date = date;
        return this;
    }
    
    public MealBuilder AddPackedIngredient(PackedDTO packedIngredient)
    {
        _packedIngredient.Append(packedIngredient);
        return this;
    }

    public MealDTO Build()
    {
        return new MealDTO
        {
            Id = _id,
            Name = _name,
            User_id = _userId,
            Image_id = _foodImageId,
            Date = _date
        };
    }
}