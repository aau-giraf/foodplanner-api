using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class PackedIngredientBuilder : PackedIngredient
{
    public PackedIngredientBuilder()
    {
        this.Id = 99999;
        this.Ingredient_id = 99999;
        this.Meal_id = 99999;
        this.order_number = 99999;
    }

    public PackedIngredientBuilder WithId(int id)
    {
        this.Id = id;
        return this;
    }

    public PackedIngredientBuilder WithIngredientId(int ingredientId)
    {
        this.Ingredient_id = ingredientId;
        return this;
    }

    public PackedIngredientBuilder WithMealId(int mealId)
    {
        this.Meal_id = mealId;
        return this;
    }

    public PackedIngredientBuilder WithOrderNumber(int orderNumber)
    {
        this.order_number = orderNumber;
        return this;
    }

    public PackedIngredient Build()
    {
        return new PackedIngredient
        {
            Id = this.Id,
            Ingredient_id = this.Ingredient_id,
            Meal_id = this.Meal_id,
            order_number = this.order_number
        };
    }
}