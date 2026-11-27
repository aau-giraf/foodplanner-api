using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

// Interface for preference repository
public interface IPreferenceRepository
{
    Task<int> InsertAsync(Preference entity);

    Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id);
}