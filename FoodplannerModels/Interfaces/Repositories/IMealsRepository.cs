using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IMealsRepository
{
    Task<IEnumerable<Meal>> GetAllByUserAndDateAsync(int id, DateOnly startDate, DateOnly endDate);
    Task<Meal?> GetByIdAsync(int id);
    Task<int> InsertAsync(MealCreateDTO entity, int userId);
    Task<int> UpdateAsync(Meal entity);
    Task<int> DeleteAsync(int id);
    Task<IEnumerable<Meal>> GetAllTemplatesByUserAsync(int userId);
    Task<int> UpdateTemplateStatusAsync(int id, bool template);
}