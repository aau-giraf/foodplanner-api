using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Meal class.
*/
public class MealService (IMealRepository mealRepository) : IMealService
{
    private readonly IMealRepository _mealRepository = mealRepository;

    public async Task<IEnumerable<Meal>> GetAllMealsAsync(){

        var meal = await _mealRepository.GetAllAsync();
        return meal;
    }

    public async Task<Meal> GetMealByIdAsync(int id){
        return await _mealRepository.GetByIdAsync(id);
    }
    
    public async Task<int> CreateMealAsync(Meal meal){
        return await _mealRepository.InsertAsync(meal);
    }
    
    public async Task<int> UpdateMealAsync(Meal meal){
        return await _mealRepository.UpdateAsync(meal);
    }

    public async Task<int> DeleteMealAsync(int id){
        return await _mealRepository.DeleteAsync(id);
    }
}