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

        var ingredient = await _ingredientRepository.GetAllAsync();
        return ingredient;
    }

    public async Task<Ingredient> GetIngredientByNameAsync(string name){
        return await _ingredientRepository.GetByNameAsync(name);
    }
    
    public async Task<int> CreateIngredientAsync(Ingredient ingredient){
        return await _ingredientRepository.InsertAsync(ingredient);
    }
}