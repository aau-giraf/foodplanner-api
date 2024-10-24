namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IMealRepository
{
    // Gets all meal asynchronously.
    Task<IEnumerable<Meal>> GetAllAsync();
    // Gets an meal by ID asynchronously.
    Task<Meal> GetByIdAsync(int id);
    // Inserts a new meal asynchronously.
    Task<int> InsertAsync(Meal entity);
    // Updates an existing meal asynchronously.
    Task<int> UpdateAsync(Meal entity, int id);
    // Deletes an meal by ID asynchronously.
    Task<int> DeleteAsync(int id);
}
