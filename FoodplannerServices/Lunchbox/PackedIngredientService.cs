using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox
{
    /**
    * Service for the PackedIngredient class.
    */
    public class PackedIngredientService(IPackedIngredientRepository packedIngredientRepository, IMapper mapper) : IPackedIngredientService
    {
        private readonly IPackedIngredientRepository _packedIngredientRepository = packedIngredientRepository;
        private readonly IMapper _mapper = mapper;

        // Get all packed ingredients
        public async Task<IEnumerable<PackedIngredientDTO>> GetAllPackedIngredientsAsync() {
            var packedIngredient = await _packedIngredientRepository.GetAllAsync();
            var packedIngredientDTO = _mapper.Map<IEnumerable<PackedIngredientDTO>>(packedIngredient);
            return packedIngredientDTO;
        }

        // Get all packed ingredients based on a meal ID
        public async Task<IEnumerable<PackedIngredientDTO>> GetAllPackedIngredientsByMealIdAsync(int id) {
            var packedIngredient = await _packedIngredientRepository.GetAllByMealIdAsync(id);
            var packedIngredientDTO = _mapper.Map<IEnumerable<PackedIngredientDTO>>(packedIngredient);
            return packedIngredientDTO;
        }

        // Get a packed ingredient based on the ID
        public async Task<PackedIngredient> GetPackedIngredientByIdAsync(int id) {
            return await _packedIngredientRepository.GetByIdAsync(id);
        }

        // Creates a new packed ingredient
        public async Task<int> CreatePackedIngredientAsync(int meal_ref, int ingredient_ref){
            return await _packedIngredientRepository.InsertAsync(meal_ref, ingredient_ref);
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