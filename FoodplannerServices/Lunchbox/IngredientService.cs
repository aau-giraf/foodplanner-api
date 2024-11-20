using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Ingredient class.
*/
public class IngredientService (IIngredientRepository ingredientRepository) : IIngredientService
{
    // Dependency injection of the ingredient repository.
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;
    // Retrieves all ingredients from the repository.
    public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync(){
        return await _ingredientRepository.GetAllAsync();
    }
    // Retrieves all ingredients by user.
    public async Task<IEnumerable<Ingredient>> GetAllIngredientsByUserAsync(int userId){

        var ingredients = await _ingredientRepository.GetAllByUserAsync(userId);
        return ingredients.ToList();
    }
    // Retrieves a specific ingredient by its ID.
    public async Task<Ingredient> GetIngredientByIdAsync(int id){
        return await _ingredientRepository.GetByIdAsync(id);
    }
    // Creates a new ingredient in the repository.
    public async Task<int> CreateIngredientAsync(Ingredient ingredient){
        return await _ingredientRepository.InsertAsync(ingredient);
    }
    // Updates an existing ingredient in the repository by ID.
    public async Task<int> UpdateIngredientAsync(Ingredient ingredient, int id){
        return await _ingredientRepository.UpdateAsync(ingredient, id);
    }
    // Deletes an ingredient from the repository by ID.
    public async Task<int> DeleteIngredientAsync(int id){
        return await _ingredientRepository.DeleteAsync(id);
    }
}