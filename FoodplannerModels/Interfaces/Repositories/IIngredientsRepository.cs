using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IIngredientsRepository
{
    Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id);
    Task<Ingredient?> GetByIdAsync(int id);
    Task<int> InsertAsync(Ingredient entity, IEnumerable<int>? categoryIds = null);
    Task<int> UpdateAsync(Ingredient entity);
    Task<int> DeleteAsync(int id);
}
