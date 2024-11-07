using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox
{
    /**
    * Service for the PackedIngredient class.
    */
    public class PackedIngredientService (IPackedIngredientRepository packedIngredientRepository) : IPackedIngredientService
    {
        private readonly IPackedIngredientRepository _packedIngredientRepository = packedIngredientRepository;

        // Get all packed ingredients
        public async Task<IEnumerable<PackedIngredient>> GetAllPackedIngredientsAsync() {
            var packedIngredients = await _packedIngredientRepository.GetAllAsync();
            return packedIngredients;
        }

        // Get all packed ingredients based on a meal ID
        public async Task<IEnumerable<PackedIngredient>> GetAllPackedIngredientsByMealIdAsync(int id) {
            var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(id);
            return packedIngredients;
        }

        // Get a packed ingredient based on the ID
        public async Task<PackedIngredient> GetPackedIngredientByIdAsync(int id) {
            return await _packedIngredientRepository.GetByIdAsync(id);
        }

        // Creates a new packed ingredient
        public async Task<int> CreatePackedIngredientAsync(PackedIngredient packedIngredient){
            return await _packedIngredientRepository.InsertAsync(packedIngredient);
        }

        // Updates an existing packed ingredient
        public async Task<int> UpdatePackedIngredientAsync(PackedIngredient packedIngredient, int id) {
            return await _packedIngredientRepository.UpdateAsync(packedIngredient, id);
        }

        // Deletes a packed ingredient based on the ID
        public async Task<int> DeletePackedIngredientAsync(int id) {
            return await _packedIngredientRepository.DeleteAsync(id);
        }
    }
}