using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

public interface IPreferenceService
{
  Task<int> CreatePreference(PreferenceDTO createPreference);  
}