using FoodplannerModels.Preferences;
using FoodplannerModels.Account;

// Interface for preference repository
public interface IPreferenceRepository
{
    Task<int> InsertAsync(Preference entity);

    Task<int> UpdateAsync(Preference entity);

    Task<int> DeleteAsync(Preference entity);

    Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id);
}