using FoodplannerModels.Account;
public interface IPreferencesService
{
  Task<int> CreatePreferences(PreferencesDTO createPreferences); 

  Task<int> UpdatePreferences(PreferencesDTO updatePreferences);
}