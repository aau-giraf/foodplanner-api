using FoodplannerModels.Account;
public interface IPreferencesService
{
  Task<int> CreatePreferencesAsync(PreferenceDTO createPreferences); 

  Task<int> UpdatePreferencesAsync(PreferenceDTO updatePreferences);

  Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id);

  Task<IEnumerable<PreferenceDTO>> GetPreferencesFromRatingAsync(string rating);
}