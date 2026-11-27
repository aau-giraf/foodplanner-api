using System.Data.Common;
using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerServices.Account;

public class PreferenceService(IPreferencesRepository preferencesRepository) : IPreferencesService
{
    // Method for creating a preference
    // Returns a preference with an ID
    public async Task<int> CreatePreferencesAsync(PreferenceDTO createPreferences)
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
    public async Task<int> UpdatePreferencesAsync(PreferenceDTO updatePreferences)
    {
        var preferences = new Preference{Id = updatePreferences.Id, Rating = updatePreferences.Rating, Food_name = updatePreferences.Food_name, Date = updatePreferences.Date, Food_image_id = updatePreferences.Food_image_id};

        return await preferencesRepository.UpdateAsync(preferences);
    }

    // Retrieves a list of preferences from child id 
    public async Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id)
    {
        try
        {
            var preferences = await preferencesRepository.GetPreferencesByChildIdAsync(id);
            return preferences;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving preferences with child ID {id}: {exception.Message}", exception);
        }
    }

    // Retrieves a flist of preferences from a rating string 
    public async Task<IEnumerable<PreferenceDTO>> GetPreferencesFromRatingAsync(string rating)
    {
        try
        {
            var preferences = await preferencesRepository.GetPreferencesFromRatingAsync(rating);
            return preferences;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving preferences with rating \"{rating}\": {exception.Message}", exception);
        }
    }
    
}