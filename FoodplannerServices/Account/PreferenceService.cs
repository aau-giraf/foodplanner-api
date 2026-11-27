using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerServices.Account;

public class PreferenceService(IPreferenceRepository preferenceRepository) : IPreferenceService
{
    // Method for creating a preference
    // Returns a preference with an ID
    public async Task<int> CreatePreference(PreferenceDTO createPreference)
    {
        var preference = new Preference
        {
           Child_id = createPreference.Child_id,
           Rating = createPreference.Rating,
           Food_name = createPreference.Food_name,
           Date = createPreference.Date, 
           Food_image_id = createPreference.Food_image_id
        };

        return await preferenceRepository.InsertAsync(preference);
    
    }

    // Retrieves a list of preferences from child id 
    public async Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id)
    {
        try
        {
            var preferences = await preferenceRepository.GetPreferencesByChildIdAsync(id);
            return preferences;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving preferences with child ID {id}: {exception.Message}", exception);
        }
    }
}