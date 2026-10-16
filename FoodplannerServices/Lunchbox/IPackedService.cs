using System.Collections.Generic;
using System.Threading.Tasks;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IPackedService
{
    Task<IEnumerable<PackedDTO>> GetAllPackedAsync();

    Task<IEnumerable<PackedDTO>> GetAllPackedByMealIdAsync(int id);

    Task<int> CreatePackedAsync(PackedDTO packed);

    Task<int> UpdatePackedAsync(PackedDTO oldPacked, PackedDTO newPacked);

    Task<int> DeletePackedAsync(PackedDTO packed);
}
