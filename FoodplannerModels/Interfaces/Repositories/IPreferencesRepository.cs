using FoodplannerModels.Preferences;
using FoodplannerModels.Account;

public interface IPreferencesRepository
{
    Task<int> InsertAsync(Preference entity);

    Task<int> UpdateAsync(Preference entity);

    Task<int> DeleteAsync(Preference entity);

    Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id);

    Task<IEnumerable<Preference>> GetRetryCandidatesAsync(
        int childId,
        string dislikedRating,
        DateTime cutoffDate,
        int limit
    );
}