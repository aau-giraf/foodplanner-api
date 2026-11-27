using FoodplannerModels.Account;
public interface IPreferencesService
{
  Task<int> CreatePreferences(PreferenceDTO createPreferences); 

  Task<int> UpdatePreferences(PreferenceDTO updatePreferences);

  Task<int> DeletePreferences(PreferenceDTO deletePreferences)
}