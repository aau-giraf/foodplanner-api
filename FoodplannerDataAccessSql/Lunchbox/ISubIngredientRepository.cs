using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface ISubIngredientRepository
{
    Task<IEnumerable<SubIngredientDTO>> GetAllAsync();

    Task<IEnumerable<SubIngredientDTO>> GetAllByParentIdAsync(int id);

    Task<int> InsertAsync(SubIngredientDTO entity);

    Task<int> UpdateAsync(SubIngredientDTO Old, SubIngredientDTO New);

    Task<int> DeleteAsync(SubIngredientDTO entity);
}
