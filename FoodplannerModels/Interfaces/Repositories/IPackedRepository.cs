using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IPackedRepository
{
    Task<IEnumerable<Packed>> GetAllByMealIdAsync(int id);
    Task<int> InsertAsync(Packed entity);
    Task<int> UpdateAsync(Packed Old, Packed New);
    Task<int> DeleteAsync(Packed entity);
}
