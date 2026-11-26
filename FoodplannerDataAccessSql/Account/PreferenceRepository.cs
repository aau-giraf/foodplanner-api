namespace FoodplannerDataAccessSql.Account;

// Adds Preference Repository to the FoodplannerDataAccessSql.Tags namespace
public class PreferenceRepository(IDapperExecutor executor) : IPreferenceRepository
{
    public async Task<int> InsertAsync
    
}