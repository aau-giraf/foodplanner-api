using System.Collections.Generic;
using System.Threading.Tasks;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IPackedService
{
    Task<int> CreatePackedAsync(Packed packed);
    Task<int> UpdatePackedAsync(Packed oldPacked, Packed newPacked);
    Task<int> DeletePackedAsync(Packed packed);
}
