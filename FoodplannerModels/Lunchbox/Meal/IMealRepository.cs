namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IMealRepository
{
    Task<IEnumerable<Meal>> GetAllAsync();
    Task<Meal> GetByNameAsync(string name);
    Task<int> InsertAsync(Meal entity);
    Task<int> UpdateAsync(Meal entity);
    Task<int> DeleteAsync(string name);
}
