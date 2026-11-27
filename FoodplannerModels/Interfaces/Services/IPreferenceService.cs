using FoodplannerModels.Account;
public interface IPreferencesService
{
  Task<int> CreatePreference(PreferencesDTO createPreference);  
}