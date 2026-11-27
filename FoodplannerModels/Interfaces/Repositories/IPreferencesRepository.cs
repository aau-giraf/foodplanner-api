using FoodplannerModels.Preferences;

// Interface for preference repository
public interface IPreferencesRepository
{
    Task<int> InsertAsync(Preferences entity);

    Task<int> UpdateAsync(Preferences entity);

    Task<IEnumerable<Preferences>> GetRetryCandidatesAsync(
        int childId,
        string dislikedRating,
        DateTime cutoffDate,
        int limit
    );
}