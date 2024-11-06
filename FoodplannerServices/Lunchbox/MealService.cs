using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Meal class.
*/
public class MealService (IMealRepository mealRepository, IPackedIngredientRepository packedIngredientRepository, IIngredientRepository ingredientRepository) : IMealService
{
    // Dependency injection of the meal repository.
    private readonly IMealRepository _mealRepository = mealRepository;
    private readonly IPackedIngredientRepository _packedIngredientRepository = packedIngredientRepository;
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;

    // Retrieves all meals from the repository.
    public async Task<List<MealDTO>> GetAllMealsAsync(){
        var meals = await _mealRepository.GetAllAsync();
        List<MealDTO> output = [];
        foreach(Meal meal in meals)
        {
            var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(meal.Id);

        List<PackedIngredientDTO> ingredients = [];
        foreach(PackedIngredient element in packedIngredients)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(element.Ingredient_ref);
            PackedIngredientDTO packed = new() {Id = element.Id, Meal_ref = element.Meal_ref, Ingredient_ref = ingredient};
            ingredients.Add(packed);
        }
        MealDTO mealDTO = new() {Id = meal.Id, Image_ref = meal.Image_ref, Title = meal.Title, Date = meal.Date, Ingredients = ingredients};
        output.Add(mealDTO);
        }

        return output;
    }

    // Retrieves all meals by user id.
    public async Task<List<MealDTO>> GetAllMealsByUserAsync(int user_ref, string date){
        var meals = await _mealRepository.GetAllByUserAsync(user_ref, date);
        List<MealDTO> output = [];
        foreach(Meal meal in meals)
        {
            var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(meal.Id);

        List<PackedIngredientDTO> ingredients = [];
        foreach(PackedIngredient element in packedIngredients)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(element.Ingredient_ref);
            PackedIngredientDTO packed = new() {Id = element.Id, Meal_ref = element.Meal_ref, Ingredient_ref = ingredient};
            ingredients.Add(packed);
        }
        MealDTO mealDTO = new() {Id = meal.Id, Image_ref = meal.Image_ref, Title = meal.Title, Date = meal.Date, Ingredients = ingredients};
        output.Add(mealDTO);
        }

        return output;
    }

    // Retrieves a specific meal by its ID.
    public async Task<MealDTO> GetMealByIdAsync(int id){
        var meal = await _mealRepository.GetByIdAsync(id);
        var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(id);

        List<PackedIngredientDTO> ingredients = [];
        foreach(PackedIngredient element in packedIngredients)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(element.Ingredient_ref);
            PackedIngredientDTO packed = new() {Id = element.Id, Meal_ref = element.Meal_ref, Ingredient_ref = ingredient};
            ingredients.Add(packed);
        }

        if(meal == null)
        {
            return null;
        }
        else
        {
            MealDTO output = new() {Id = meal.Id, Image_ref = meal.Image_ref, Title = meal.Title, Date = meal.Date, Ingredients = ingredients};
            return output;
        }
    }

    // Creates a new meal in the repository.
    public async Task<int> CreateMealAsync(Meal meal){
        return await _mealRepository.InsertAsync(meal);
    }
    // Updates an existing meal in the repository by ID.
    
    public async Task<int> UpdateMealAsync(Meal meal, int id){
        return await _mealRepository.UpdateAsync(meal, id);
    }
    // Deletes an meal from the repository by ID.

    public async Task<int> DeleteMealAsync(int id){
        return await _mealRepository.DeleteAsync(id);
    }
}