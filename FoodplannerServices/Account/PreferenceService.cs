using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerServices.Account;

public class PreferenceService(IPreferencesRepository preferencesRepository) : IPreferencesService
{
    // Method for creating a preference
    // Returns a preference with an ID
    public async Task<int> CreatePreference(PreferencesDTO createPreference)
    {
        var preference = new Preferences
        {
           Child_id = createPreference.Child_id,
           Rating = createPreference.Rating,
           Food_name = createPreference.Food_name,
           Date = createPreference.Date, 
           Food_image_id = createPreference.Food_image_id
        };

        return await preferencesRepository.InsertAsync(preference);
    
    }

    public async Task<int> UpdatePreference(PreferencesDTO entity)
}