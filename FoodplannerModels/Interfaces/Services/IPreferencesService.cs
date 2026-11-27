using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

public interface IPreferencesService
{
    Task<int> CreatePreferences(PreferencesDTO createPreferences);

    Task<int> UpdatePreferences(PreferencesDTO updatePreferences);

    Task<IEnumerable<Preferences>> RecommendRetryAsync(int childId);
}