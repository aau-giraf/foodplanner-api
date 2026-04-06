namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IMealRepository : IGenericRepository<Meal>
{
    // Gets all meals by user id asynchronously.
    Task<IEnumerable<Meal>> GetAllByUserAsync(int id, string date);
    // Get all meals that have template set to 1
    Task<IEnumerable<Meal>> GetAllTemplatesByUserAsync(int userId);
    // Update a meals template status by its ID
    Task<int> UpdateTemplateStatusAsync(int id, bool template);
}
