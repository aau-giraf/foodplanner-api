using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/**
* The service for the Meal class.
*/
public class MealService (IMealRepository mealRepository, IPackedIngredientRepository packedIngredientRepository, IIngredientRepository ingredientRepository, IMapper mapper) : IMealService
{
    // Dependency injection of the meal repository.
    private readonly IMealRepository _mealRepository = mealRepository;
    private readonly IPackedIngredientRepository _packedIngredientRepository = packedIngredientRepository;
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;
    private readonly IMapper _mapper;

    // Retrieves all meals from the repository.
    public async Task<IEnumerable<MealDTO>> GetAllMealsAsync(){
        var meal = await _mealRepository.GetAllAsync();
        var mealDTO = _mapper.Map<IEnumerable<MealDTO>>(meal);
        return mealDTO;
    }

    // Retrieves all meals by user id.
    public async Task<IEnumerable<MealDTO>> GetAllMealsByUserAsync(int user_ref, string date){
        var meal = await _mealRepository.GetAllByUserAsync(user_ref, date);
        var mealDTO = _mapper.Map<IEnumerable<MealDTO>>(meal);
        return mealDTO;
    }

    // Retrieves a specific meal by its ID.
    public async Task<MealDTO> GetMealByIdAsync(int id){
        var meal = await _mealRepository.GetByIdAsync(id);
        return _mapper.Map<MealDTO>(meal);
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