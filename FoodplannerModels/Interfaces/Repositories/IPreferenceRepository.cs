public interface IPreferenceRepository
{
    Task<int> InsertAsync(Preference entity);
}