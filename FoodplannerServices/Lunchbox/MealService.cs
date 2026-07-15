using FoodplannerModels.Lunchbox;
using AutoMapper;


namespace FoodplannerServices.Lunchbox;

/**
* The service for the Meal class.
*/
public class MealService(IMealRepository mealRepository, IPackedIngredientRepository packedIngredientRepository, IIngredientRepository ingredientRepository, IMapper mapper) : IMealService
{
    // Dependency injection of the meal repository.
    private readonly IMealRepository _mealRepository = mealRepository;
    private readonly IPackedIngredientRepository _packedIngredientRepository = packedIngredientRepository;
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;
    private readonly IMapper _mapper = mapper;

    // Maps a packed ingredient to its response shape, nesting the ingredient's
    // details (from the pre-fetched lookup) under "ingredient_id".
    private static PackedIngredientResponseDTO ToResponse(PackedIngredient p, IDictionary<int, Ingredient?> ingredientsById)
    {
        ingredientsById.TryGetValue(p.Ingredient_id, out var ingredient);
        return new PackedIngredientResponseDTO
        {
            Id = p.Id,
            Meal_id = p.Meal_id,
            order_number = p.order_number,
            Ingredient = ingredient is not null
                ? new IngredientDTO
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    User_id = ingredient.User_id,
                    Food_image_id = ingredient.Food_image_id
                }
                : new IngredientDTO { Id = p.Ingredient_id, Name = string.Empty, User_id = 0, Food_image_id = null }
        };
    }


    // Builds the nested-ingredient response for a set of meals: fetches each
    // meal's packed ingredients and the referenced ingredient details, then
    // projects to MealResponseDTO.
    private async Task<List<MealResponseDTO>> MapMealsToResponsesAsync(IReadOnlyCollection<Meal> meals)
    {
        // Fetch all packed ingredients for all meals in one go.
        var packedIngredientsByMeal = await Task.WhenAll(
            meals.Select(async meal =>
            {
                var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(meal.Id);
                return new { meal.Id, PackedIngredients = packedIngredients };
            })
        );

        // Fetch each referenced ingredient's details once.
        var ingredientIds = packedIngredientsByMeal
            .SelectMany(m => m.PackedIngredients)
            .Select(p => p.Ingredient_id)
            .Distinct()
            .ToList();
        var ingredientsById = (await Task.WhenAll(
            ingredientIds.Select(async id =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(id);
                return new { Id = id, Ingredient = ingredient };
            })
        )).ToDictionary(i => i.Id, i => i.Ingredient);

        return meals.Select(meal => new MealResponseDTO
        {
            Id = meal.Id,
            Food_image_id = meal.Food_image_id,
            Name = meal.Name,
            Date = meal.Date,
            Template = meal.Template,
            UserId = meal.User_id,
            Ingredients = packedIngredientsByMeal
                .First(m => m.Id == meal.Id).PackedIngredients
                .Select(p => ToResponse(p, ingredientsById))
                .ToList()
        }).ToList();
    }

    // Retrieves all meals from the repository.
    public async Task<IEnumerable<MealResponseDTO>> GetAllMealsAsync()
        => await MapMealsToResponsesAsync((await _mealRepository.GetAllAsync()).ToList());


    // Retrieves all meals by user id.
    public async Task<IEnumerable<MealResponseDTO>> GetAllMealsByUserAsync(int userId, string date)
        => await MapMealsToResponsesAsync((await _mealRepository.GetAllByUserAsync(userId, date)).ToList());

    // Retrieves a specific meal by its ID.
    public async Task<MealResponseDTO?> GetMealByIdAsync(int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
            return null;

        var responses = await MapMealsToResponsesAsync(new[] { meal });
        return responses.FirstOrDefault();
    }


    // Creates a new meal in the repository.
    public async Task<int> CreateMealAsync(MealCreateDTO mealCreateDTO, int id)
    {
        var meal = _mapper.Map<Meal>(mealCreateDTO);
        meal.User_id = id;
        return await _mealRepository.InsertAsync(meal);
    }
    // Updates an existing meal in the repository by ID.

    public async Task<int> UpdateMealAsync(MealDTO mealDto, int id)
    {
        var meal = _mapper.Map<Meal>(mealDto);
        meal.Id = id;
        var result = _mealRepository.UpdateAsync(meal);
        
        return await result;
    }
    // Deletes an meal from the repository by ID.

    public async Task<int> DeleteMealAsync(int id)
    {
        return await _mealRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<MealResponseDTO>> GetAllTemplatesByUserAsync(int userId)
        => await MapMealsToResponsesAsync((await _mealRepository.GetAllTemplatesByUserAsync(userId)).ToList());

    // Update this method with ownership verification:
    public async Task<int> UpdateTemplateStatusAsync(int id, bool template, int userId)
    {
        // Verify ownership
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
        {
            throw new InvalidOperationException("Måltid ikke fundet");
        }
        
        if (meal.User_id != userId)
        {
            throw new InvalidOperationException("Du har ikke tilladelse til at ændre dette måltid");
        }
        
        return await _mealRepository.UpdateTemplateStatusAsync(id, template);
    }

    // Update this method with ownership verification:
    public async Task<IEnumerable<Ingredient>> GetUniqueIngredientsFromMealsAsync(List<int> mealIds, int userId)
    {
        // Verify all meals belong to the user
        foreach (var mealId in mealIds)
        {
            var meal = await _mealRepository.GetByIdAsync(mealId);
            if (meal == null || meal.User_id != userId)
            {
                throw new InvalidOperationException("Du har ikke tilladelse til at få adgang til alle de angivne måltider");
            }
        }

        var allIngredients = new Dictionary<int, Ingredient>();

        foreach (var mealId in mealIds)
        {
            var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(mealId);
            
            foreach (var packedIngredient in packedIngredients)
            {
                if (!allIngredients.ContainsKey(packedIngredient.Ingredient_id))
                {
                    var ingredient = await _ingredientRepository.GetByIdAsync(packedIngredient.Ingredient_id);
                    if (ingredient != null)
                    {
                        allIngredients[packedIngredient.Ingredient_id] = ingredient;
                    }
                }
            }
        }

        return allIngredients.Values;
    }

}