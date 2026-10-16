using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IPackedRepository
{
    Task<IEnumerable<PackedDTO>> GetAllAsync();

    Task<IEnumerable<PackedDTO>> GetAllByMealIdAsync(int id);

    Task<int> InsertAsync(PackedDTO entity);

    Task<int> UpdateAsync(PackedDTO Old, PackedDTO New);

    Task<int> DeleteAsync(PackedDTO entity);
}
