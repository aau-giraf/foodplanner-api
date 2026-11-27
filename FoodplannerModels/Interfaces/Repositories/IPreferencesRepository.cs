using FoodplannerModels.Preferences;

// Interface for preference repository
public interface IPreferencesRepository
{
    Task<int> InsertAsync(Preferences entity);

    Task<int> UpdateAsync(Preferences entity);

    Task<int> DeleteAsync(Preferences entity);
}