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

    public async Task<Ingredient> GetIngredientByIdAsync(int id){
        return await _ingredientRepository.GetByIdAsync(id);
    }
    
    public async Task<int> CreateIngredientAsync(Ingredient ingredient){
        return await _ingredientRepository.InsertAsync(ingredient);
    }
    
    public async Task<int> UpdateIngredientAsync(Ingredient ingredient, int id){
        return await _ingredientRepository.UpdateAsync(ingredient, id);
    }

    public async Task<int> DeleteIngredientAsync(int id){
        return await _ingredientRepository.DeleteAsync(id);
    }
}