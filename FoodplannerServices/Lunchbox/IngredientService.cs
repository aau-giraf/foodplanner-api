using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Ingredient class.
*/
public class IngredientService(IIngredientRepository ingredientRepository) : IIngredientService
{
    private readonly IMapper _mapper;
    // Dependency injection of the ingredient repository.
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;
    // Retrieves all ingredients from the repository.
    public async Task<IEnumerable<IngredientDTO>> GetAllIngredientsAsync()
    { 
        var ingredients = await _ingredientRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<IngredientDTO>>(ingredients);
    }
    // Retrieves all ingredients by user.
    public async Task<IEnumerable<IngredientDTO>> GetAllIngredientsByUserAsync(int userId)
    {
        var ingredients = await _ingredientRepository.GetAllByUserAsync(userId);
        return _mapper.Map<IEnumerable<IngredientDTO>>(ingredients).ToList();
    }
    // Retrieves a specific ingredient by its ID.
    public async Task<IngredientDTO> GetIngredientByIdAsync(int id)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id);
        return _mapper.Map<IngredientDTO>(ingredient);
    }
    // Creates a new ingredient in the repository.
    public async Task<int> CreateIngredientAsync(IngredientDTO ingredientDto, int id)
    {
        return await _ingredientRepository.InsertAsync(ingredientDto, id);
    }
    // Updates an existing ingredient in the repository by ID.
    public async Task<int> UpdateIngredientAsync(IngredientDTO ingredientDto, int id)
    {
        var ingredient = _mapper.Map<Ingredient>(ingredientDto);
        return await _ingredientRepository.UpdateAsync(ingredient, id);
    }
    // Deletes an ingredient from the repository by ID.
    public async Task<int> DeleteIngredientAsync(int id)
    {
        return await _ingredientRepository.DeleteAsync(id);
    }
}