﻿namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the meal.
*/
public interface IMealService
{
    Task<IEnumerable<MealDTO>> GetAllMealsAsync();
    Task<Meal> GetMealByNameAsync(string name);
    Task<int> CreateMealAsync(Meal meal);
    Task<int> UpdateMealAsync(Meal meal);
    Task<int> DeleteMealAsync(string name);
}