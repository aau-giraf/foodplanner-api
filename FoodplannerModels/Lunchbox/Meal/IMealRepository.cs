namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IMealRepository : IGenericRepository<Meal>
{
    // Gets all meals by user id asynchronously.
    Task<IEnumerable<Meal>> GetAllByUserAsync(int id, string date);
}
