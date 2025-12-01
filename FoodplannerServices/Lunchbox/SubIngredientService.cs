using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/// <summary>
/// Service implementation for SubIngredient business logic
/// </summary>
public class SubIngredientService : ISubIngredientService
{
    private readonly ISubIngredientRepository _subIngredientRepository;

    public SubIngredientService(ISubIngredientRepository subIngredientRepository)
    {
        _subIngredientRepository = subIngredientRepository;
    }

    public async Task<IEnumerable<SubIngredient>> GetAllSubIngredientsAsync()
    {
        return await _subIngredientRepository.GetAllAsync();
    }

    public async Task<IEnumerable<SubIngredient>> GetAllSubIngredientsByUserAsync(int userId)
    {
        var subingredients = await _subIngredientRepository.GetAllByUserAsync(userId);
        return subingredients.ToList();
    }

    public async Task<SubIngredient> GetSubIngredientByIdAsync(int id)
    {
        return await _subIngredientRepository.GetByIdAsync(id);
    }

    public async Task<int> CreateSubIngredientAsync(SubIngredientDTO subingredient, int userId)
    {
        return await _subIngredientRepository.InsertAsync(subingredient, userId);
    }

    public async Task<int> UpdateSubIngredientAsync(SubIngredient subingredient, int id)
    {
        return await _subIngredientRepository.UpdateAsync(subingredient, id);
    }

    public async Task<int> DeleteSubIngredientAsync(int id)
    {
        return await _subIngredientRepository.DeleteAsync(id);
    }
}