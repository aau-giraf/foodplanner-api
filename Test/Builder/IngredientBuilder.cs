using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class IngredientBuilder
{
    private int _id = 99999;
    private string _name = "testName";
    private int _userId = 99999;
    private int? _foodImageId;

    public IngredientBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public IngredientBuilder WithImageId(int id)
    {
        _foodImageId = id;
        return this;
    }

    public IngredientBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public IngredientBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public Ingredient Build()
    {
        return new Ingredient
        {
            Id = _id,
            Name = _name,
            User_id = _userId,
            Food_image_id = _foodImageId,
        };
    }
}