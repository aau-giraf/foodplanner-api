using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Meal class.
*/
public class MealService (IMealRepository mealRepository) : IMealService
{
    // Dependency injection of the meal repository.
    private readonly IMealRepository _mealRepository = mealRepository;
    // Retrieves all meals from the repository.

    public async Task<IEnumerable<Meal>> GetAllMealsAsync(){

        var meal = await _mealRepository.GetAllAsync();
        return meal;
    }

// Retrieves a specific meal by its ID.
    public async Task<Meal> GetMealByIdAsync(int id){
        return await _mealRepository.GetByIdAsync(id);
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