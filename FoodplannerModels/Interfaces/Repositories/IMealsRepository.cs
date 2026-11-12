using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IMealsRepository
{
    Task<IEnumerable<MealDTO>> GetAllAsync();

    Task<IEnumerable<MealDTO>> GetAllByUserAndDateAsync(int id, string date);

    Task<MealDTO?> GetByIdAsync(int id);

    Task<int> InsertAsync(MealCreateDTO entity, int userId);

    Task<int> UpdateAsync(MealDTO entity);

    Task<int> DeleteAsync(int id);

    Task<IEnumerable<MealDTO>> GetAllTemplatesByUserAsync(int userId);

    Task<int> UpdateTemplateStatusAsync(int id, bool template);

}