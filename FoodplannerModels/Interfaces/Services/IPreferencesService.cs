using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

public interface IPreferencesService
{
    Task<int> CreatePreferencesAsync(PreferenceDTO createPreferences);

    Task<int> UpdatePreferencesAsync(PreferenceDTO updatePreferences);

    Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id);

    Task<IEnumerable<Preference>> RecommendRetryAsync(int childId);
}