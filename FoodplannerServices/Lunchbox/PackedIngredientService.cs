using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox
{
    /**
    * Service for the PackedIngredient class.
    */
    public class PackedIngredientService (IPackedIngredientRepository packedIngredientRepository, IMapper mapper) : IPackedIngredientService
    {
        private readonly IPackedIngredientRepository _packedIngredientRepository = packedIngredientRepository;
        private readonly IMapper _mapper = mapper;

        // Get all packed ingredients and maps them to DTOs version 1
        public async Task<IEnumerable<PackedIngredientDTO>> GetAllPackedIngredientsAsync() {
            var packedIngredients = await _packedIngredientRepository.GetAllAsync();
            var packedIngredientsDTO = _mapper.Map<IEnumerable<PackedIngredientDTO>>(packedIngredients);
            return packedIngredientsDTO;
        }

        // Get all packed ingredients version 2
        public async Task<IEnumerable<PackedIngredient>> GetAllPackedIngredientsAsync() {
            var packedIngredients = await _packedIngredientRepository.GetAllAsync();
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

        // Updates an existing packed ingredient (not implemented)
        public async Task<int> UpdatePackedIngredientAsync(PackedIngredient packedIngredient) {
            return await _packedIngredientRepository.UpdateAsync(packedIngredient);
        }

        // Deletes a packed ingredient based on the ID
        public async Task<int> DeletePackedIngredientAsync(int id) {
            return await _packedIngredientRepository.DeleteAsync(id);
        }
    }
}
