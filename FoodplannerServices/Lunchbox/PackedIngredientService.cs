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
        public async Task<IEnumerable<PackedIngredientProperDTO>> GetAllPackedIngredientsAsync()
        {
            var packedIngredient = await _packedIngredientRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PackedIngredientProperDTO>>(packedIngredient);
        }

        // Get all packed ingredients based on a meal ID
        public async Task<IEnumerable<PackedIngredientDTO>> GetAllPackedIngredientsByMealIdAsync(int id)
        {
            var packedIngredient = await _packedIngredientRepository.GetAllByMealIdAsync(id); 
            return _mapper.Map<IEnumerable<PackedIngredientDTO>>(packedIngredient);
        }

        // Get a packed ingredient based on the ID
        public async Task<PackedIngredientDTO> GetPackedIngredientByIdAsync(int id)
        {
            var packedIngredient = await _packedIngredientRepository.GetByIdAsync(id);
            return _mapper.Map<PackedIngredientDTO>(packedIngredient);
        }

        // Creates a new packed ingredient
        public async Task<int> CreatePackedIngredientAsync(PackedIngredientProperDTO packedIngredientProperDTO)
        {
            var packedIngredient = _mapper.Map<PackedIngredient>(packedIngredientProperDTO);
            return await _packedIngredientRepository.InsertAsync(packedIngredient);
        }

        // Updates an existing packed ingredient
        public async Task<int> UpdatePackedIngredientAsync(PackedIngredientDTO packedIngredientDto, int id)
        {
            var packedIngredient = _mapper.Map<PackedIngredient>(packedIngredientDto);
            packedIngredient.Id = id;
            return await _packedIngredientRepository.UpdateAsync(packedIngredient);
        }

        // Deletes a packed ingredient based on the ID
        public async Task<int> DeletePackedIngredientAsync(int id)
        {
            return await _packedIngredientRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdatePackedIngredientOrderAsync(List<PackedIngredientDTO> packedIngredientsDto)
        {
            var packedIngredientList = _mapper.Map<List<PackedIngredient>>(packedIngredientsDto);
            foreach (var packedIngredient in packedIngredientList)
            {
                var result = await _packedIngredientRepository.UpdateOrderAsync(packedIngredient.Id, packedIngredient.order_number);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
    }
}