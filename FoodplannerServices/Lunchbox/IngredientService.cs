using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Ingredient class.
*/
public class IngredientService (IIngredientRepository ingredientRepository) : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;

    public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync(){

        var ingredients = await _ingredientRepository.GetAllAsync();
        return ingredients;
    }

    public async Task<Ingredient> GetIngredientByNameAsync(string name, string user){
        return await _ingredientRepository.GetByNameAsync(name, user);
    }
    
    public async Task<int> CreateIngredientAsync(Ingredient meal){
        return await _ingredientRepository.InsertAsync(meal);
    }
    
    public async Task<int> UpdateIngredientAsync(Ingredient meal, string name, string user){
        return await _ingredientRepository.UpdateAsync(meal, name, user);
    }

    public async Task<int> DeleteIngredientAsync(string name, string user){
        return await _ingredientRepository.DeleteAsync(name, user);
    }
}