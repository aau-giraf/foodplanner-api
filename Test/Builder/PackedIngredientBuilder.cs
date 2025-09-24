using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class PackedIngredientBuilder
{
    private int _id = 99999;
    private int _ingredientId = 99999;
    private int _mealId = 99999;
    private int _orderNumber = 99999;

    public PackedIngredientBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public PackedIngredientBuilder WithIngredientId(int ingredientId)
    {
        _ingredientId = ingredientId;
        return this;
    }

    public PackedIngredientBuilder WithMealId(int mealId)
    {
        _mealId = mealId;
        return this;
    }

    public PackedIngredientBuilder WithOrderNumber(int orderNumber)
    {
        _orderNumber = orderNumber;
        return this;
    }

    public PackedIngredient Build()
    {
        return new PackedIngredient
        {
            Id = _id,
            Ingredient_id = _ingredientId,
            Meal_id = _mealId,
            order_number = _orderNumber
        };
    }
}