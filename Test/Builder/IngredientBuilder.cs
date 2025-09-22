using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class IngredientBuilder : Ingredient
{

    public IngredientBuilder()
    {
        this.Name = "testName";
        this.Id = 99999;
        this.User_id = 99999;
    }

    public IngredientBuilder WithName(string name)
    {
        this.Name = name;
        return this;
    }

    public IngredientBuilder WithImageId(int id)
    {
        this.Food_image_id = id;
        return this;
    }

    public Ingredient Build()
    {
        return new Ingredient
        {
            Name = this.Name,
            Food_image_id = this.Food_image_id,
            Id = this.Id,
            User_id = this.User_id,
        };
    }
}