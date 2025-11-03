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


    // Retrieves all meals from the repository.
    public async Task<IEnumerable<MealDTO>> GetAllMealsAsync()
    {
        var meals = await _mealRepository.GetAllAsync();

        // Fetch all packed ingredients for all meals in one go.
        var packedIngredientsByMeal = await Task.WhenAll(
            meals.Select(async meal =>
            {
                var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(meal.Id);
                return new { meal.Id, PackedIngredients = packedIngredients };
            })
        );

        // Fetch all ingredient details in one go.
        var allPackedIngredients = packedIngredientsByMeal.SelectMany(m => m.PackedIngredients).ToList();
        var allIngredientIds = allPackedIngredients.Select(p => p.Ingredient_id).Distinct().ToList();
        var ingredientsById = (await Task.WhenAll(
            allIngredientIds.Select(async id =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(id);
                return new { Id = id, Ingredient = ingredient };
            })
        )).ToDictionary(i => i.Id, i => i.Ingredient);

        // Construct MealDTO list.
        var output = meals.Select(meal =>
        {
            var packedIngredients = packedIngredientsByMeal
                .First(m => m.Id == meal.Id).PackedIngredients
                .Select(p => new PackedIngredientDTO
                {
                    Id = p.Id,
                    Meal_id = p.Meal_id,
                    Ingredient_id = ingredientsById[p.Ingredient_id],
                    order_number = p.order_number
                }).ToList();

            return new MealDTO
            {
                Id = meal.Id,
                Food_image_id = meal.Food_image_id,
                Name = meal.Name,
                Date = meal.Date,
                Ingredients = packedIngredients
            };
        }).ToList();

        return output;
    }


    // Retrieves all meals by user id.
    public async Task<IEnumerable<MealDTO>> GetAllMealsByUserAsync(int userId, string date)
    {
        var meals = await _mealRepository.GetAllByUserAsync(userId, date);

        // Fetch all packed ingredients for all meals in one go.
        var packedIngredientsByMeal = await Task.WhenAll(
            meals.Select(async meal =>
            {
                var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(meal.Id);
                return new { meal.Id, PackedIngredients = packedIngredients };
            })
        );

        // Fetch all ingredient details in one go.
        var allPackedIngredients = packedIngredientsByMeal.SelectMany(m => m.PackedIngredients).ToList();
        var allIngredientIds = allPackedIngredients.Select(p => p.Ingredient_id).Distinct().ToList();
        var ingredientsById = (await Task.WhenAll(
            allIngredientIds.Select(async id =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(id);
                return new { Id = id, Ingredient = ingredient };
            })
        )).ToDictionary(i => i.Id, i => i.Ingredient);

        // Construct MealDTO list.
        var output = meals.Select(meal =>
        {
            var packedIngredients = packedIngredientsByMeal
                .First(m => m.Id == meal.Id).PackedIngredients
                .Select(p => new PackedIngredientDTO
                {
                    Id = p.Id,
                    Meal_id = p.Meal_id,
                    Ingredient_id = ingredientsById[p.Ingredient_id],
                    order_number = p.order_number
                }).ToList();

            return new MealDTO
            {
                Id = meal.Id,
                Food_image_id = meal.Food_image_id,
                Name = meal.Name,
                Date = meal.Date,
                Ingredients = packedIngredients
            };
        }).ToList();

        return output;
    }

    // Retrieves a specific meal by its ID.
    public async Task<MealDTO?> GetMealByIdAsync(int id)
    {
        // Fetch the single meal.
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
            return null; // Handle the case where the meal doesn't exist.

        // Fetch all packed ingredients for the meal.
        var packedIngredients = await _packedIngredientRepository.GetAllByMealIdAsync(meal.Id);

        // Fetch all ingredient details in one go.
        var ingredientIds = packedIngredients.Select(p => p.Ingredient_id).Distinct().ToList();
        var ingredientsById = (await Task.WhenAll(
            ingredientIds.Select(async id =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(id);
                return new { Id = id, Ingredient = ingredient };
            })
        )).ToDictionary(i => i.Id, i => i.Ingredient);

        // Construct the list of PackedIngredientDTO.
        var packedIngredientDTOs = packedIngredients.Select(p => new PackedIngredientDTO
        {
            Id = p.Id,
            Meal_id = p.Meal_id,
            Ingredient_id = ingredientsById[p.Ingredient_id],
            order_number = p.order_number
        }).ToList();

        // Construct and return the MealDTO.
        return new MealDTO
        {
            Id = meal.Id,
            Food_image_id = meal.Food_image_id,
            Name = meal.Name,
            Date = meal.Date,
            Ingredients = packedIngredientDTOs
        };
    }


    // Creates a new meal in the repository.
    public async Task<int> CreateMealAsync(MealCreateDTO mealCreateDTO, int id)
    {
        var meal = _mapper.Map<Meal>(mealCreateDTO);

        return await _mealRepository.InsertAsync(meal, id);
    }
    // Updates an existing meal in the repository by ID.

    public async Task<int> UpdateMealAsync(Meal meal, int id)
    {
        return await _mealRepository.UpdateAsync(meal, id);
    }
    // Deletes an meal from the repository by ID.

    public async Task<int> DeleteMealAsync(int id)
    {
        return await _mealRepository.DeleteAsync(id);
    }
}