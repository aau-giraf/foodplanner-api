using System.Data.Common;
using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerServices.Account;

public class PreferenceService(IPreferencesRepository preferencesRepository) : IPreferencesService
{
    // Method for creating a preference
    // Returns a preference with an ID
    public async Task<int> CreatePreferences(PreferenceDTO createPreferences)
    {
        var preferences = new Preference
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
    public async Task<int> UpdatePreferences(PreferenceDTO updatePreferences)
    {
        var preferences = new Preference
        {
            Id = updatePreferences.Id, 
            Rating = updatePreferences.Rating, 
            Food_name = updatePreferences.Food_name, 
            Date = updatePreferences.Date, 
            Food_image_id = updatePreferences.Food_image_id
        };

        return await preferencesRepository.UpdateAsync(preferences);
    }

    public async Task<int> DeletePreferences(PreferenceDTO deletePreferences)
    {
        var preferences = new Preference
        { 
            Id = deletePreferences.Id, 
            Child_id = deletePreferences.Child_id,
            Rating = deletePreferences.Rating,
            Food_name = deletePreferences.Food_name,
            Date = deletePreferences.Date, 
            Food_image_id = deletePreferences.Food_image_id
        };

        return await preferencesRepository.DeleteAsync(preferences);
    }
}