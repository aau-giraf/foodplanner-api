using FoodplannerModels.Preferences;
using FoodplannerModels.Account;

// Interface for preference repository
public interface IPreferencesRepository
{
    Task<int> InsertAsync(Preference entity);

    Task<int> UpdateAsync(Preference entity);

    Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id);
}