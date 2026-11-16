using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IIngredientsRepository
{
    Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id);
    Task<Ingredient?> GetByIdAsync(int id);
    Task<int> InsertAsync(Ingredient entity);
    Task<int> UpdateAsync(Ingredient entity);
    Task<int> DeleteAsync(int id);
}
