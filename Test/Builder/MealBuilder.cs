using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class MealBuilder
{
    private int _id = 99999;
    private string _name = "testMeal";
    private string _date = "11/11/2050";
    private int _userId;
    private int? _foodImageId;

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

    public Meal Build()
    {
        return new Meal
        {
            Id = _id,
            Name = _name,
            User_id = _userId,
            Food_image_id = _foodImageId,
            Date = _date,
        };
    }
}