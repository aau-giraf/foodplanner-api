using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public class MealService (IMealRepository mealRepository, IMapper mapper) : IMealService
{
    private readonly IMealRepository _mealRepository = mealRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<MealDTO>> GetAllMealsAsync(){

        var meal = await _mealRepository.GetAllAsync();
        var mealDTO = _mapper.Map<IEnumerable<MealDTO>>(meal);
        return mealDTO;
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

    public async Task<int> DeleteMealAsync(string name){
        return await _mealRepository.DeleteAsync(name);
    }
}