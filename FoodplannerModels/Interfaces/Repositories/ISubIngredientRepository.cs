using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface ISubIngredientRepository
{
    Task<IEnumerable<SubIngredient>> GetAllByParentIdAsync(int id);
    Task<int> InsertAsync(SubIngredient entity);
    Task<int> UpdateAsync(SubIngredient Old, SubIngredient New);
    Task<int> DeleteAsync(SubIngredient entity);
}
