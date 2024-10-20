namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IMealRepository
{
    Task<IEnumerable<Meal>> GetAllAsync();
    Task<Meal> GetByIdAsync(int id);
    Task<int> InsertAsync(Meal entity);
    Task<int> UpdateAsync(Meal entity, int id);
    Task<int> DeleteAsync(int id);
}
