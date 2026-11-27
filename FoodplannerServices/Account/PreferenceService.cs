using System.Data.Common;
using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerServices.Account;

public class PreferenceService(IPreferencesRepository preferencesRepository) : IPreferencesService
{
    // Method for creating a preference
    // Returns a preference with an ID
    public async Task<int> CreatePreferences(PreferencesDTO createPreferences)
    {
        var preferences = new Preferences
        {
           Child_id = createPreferences.Child_id,
           Rating = createPreferences.Rating,
           Food_name = createPreferences.Food_name,
           Date = createPreferences.Date, 
           Food_image_id = createPreferences.Food_image_id
        };

        return await preferencesRepository.InsertAsync(preferences);
    
    }

    // Method for updating preferences 
    // Returns updated preferences with preference ID
    public async Task<int> UpdatePreferences(PreferencesDTO updatePreferences)
    {
        var preferences = new Preferences{Id = updatePreferences.Id, Rating = updatePreferences.Rating, Food_name = updatePreferences.Food_name, Date = updatePreferences.Date, Food_image_id = updatePreferences.Food_image_id};

        return await preferencesRepository.UpdateAsync(preferences);
    }
}