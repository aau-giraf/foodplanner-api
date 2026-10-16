using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

// Adds the MealService class to the FoodplannerServices.Lunchbox namespace
namespace FoodplannerServices.Lunchbox;

// The service for the Meal class.
public class MealsService(MealsRepository mealsRepository, PackedRepository packedRepository, IngredientsRepository ingredientRepository)
{
    // Retrieves all meals and their ingredients from the repository.
    public async Task<IEnumerable<MealWithIngredientsDTO>> GetAllMealsAsync()
    {
        try
        {
            // Fetch all meals and their packed ingredients.
            var meals = await mealsRepository.GetAllAsync();
            var packed = await packedRepository.GetAllAsync();
            var ingredients = await ingredientRepository.GetAllAsync();
            var ingredientsById = ingredients.ToDictionary(i => i.Id);

            // Construct and return the list of MealWithIngredientsDTO.
            var mealsWithIngredients = meals.Select(meal => new MealWithIngredientsDTO
            {
                Id = meal.Id,
                Name = meal.Name,
                Date = meal.Date,
                Template = meal.Template,
                Image_id = meal.Image_id,
                User_id = meal.User_id,

                Ingredients = packed
                    .Where(p => p.Meal_id == meal.Id)
                    .OrderBy(p => p.order_no)
                    .Select(p => ingredientsById[p.Ingredient_id])
                    .ToList()
            }).ToList();
            return mealsWithIngredients;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving all meals: {exception.Message}", exception);
        }
    }

    // Retrieves all meals and their ingredients from a user id.
    public async Task<IEnumerable<MealWithIngredientsDTO>> GetAllMealsByUserAndDateAsync(int userId, string date)
    {
        try
        {
            // Fetch all meals for the given user and date.
            var meals = await mealsRepository.GetAllByUserAndDateAsync(userId, date);
            var mealsWithIngredients = new List<MealWithIngredientsDTO>();

            // For each meal, fetch its packed ingredients and the corresponding ingredient details.
            foreach (var meal in meals)
            {
                mealsWithIngredients.Add(await AddIngredientsToMealAsync(meal));
            }
            return mealsWithIngredients;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving meals for user ID {userId} on date {date}: {exception.Message}", exception);
        }
    }

    // Retrieves a specific meal by its ID.
    public async Task<MealWithIngredientsDTO?> GetMealByIdAsync(int id)
    {
        try
        {
            // Fetch the single meal.
            var meal = await mealsRepository.GetByIdAsync(id);
            if (meal == null)
                return null;

            // Add the ingredients to the meal and return it.
            return await AddIngredientsToMealAsync(meal);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving meal with ID {id}: {exception.Message}", exception);
        }
    }

    // Creates a new meal in the repository.
    public async Task<int> CreateMealAsync(MealCreateDTO mealCreateDTO, int userId)
    {
        try
        {
            return await mealsRepository.InsertAsync(mealCreateDTO, userId);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error creating meal: {exception.Message}", exception);
        }
    }

    // Updates an existing meal in the repository by ID.
    public async Task<int> UpdateMealAsync(MealDTO mealDTO)
    {
        try
        {
            return await mealsRepository.UpdateAsync(mealDTO);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error updating meal: {exception.Message}", exception);
        }
    }

    // Deletes an meal from the repository by ID.
    public async Task<int> DeleteMealAsync(int id)
    {
        try
        {
            return await mealsRepository.DeleteAsync(id);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error deleting meal: {exception.Message}", exception);
        }
    }

    public async Task<IEnumerable<MealWithIngredientsDTO>> GetAllTemplatesByUserAsync(int userId)
    {
        try
        {
            // Fetch all meals that are templates for the given user.
            var meals = await mealsRepository.GetAllTemplatesByUserAsync(userId);
            var mealsWithIngredients = new List<MealWithIngredientsDTO>();

            // Add ingredients to each meal and return the list of meals with their ingredients.
            foreach (var meal in meals)
            {
                mealsWithIngredients.Add(await AddIngredientsToMealAsync(meal));
            }
            return mealsWithIngredients;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving template meals for user ID {userId}: {exception.Message}", exception);
        }
    }

    // Updates the template status of a meal
    public async Task<int> UpdateTemplateStatusAsync(int id, bool template, int userId)
    {
        try
        {
            // Fetch meal and verify ownership before updating template status
            var meal = await mealsRepository.GetByIdAsync(id);
            if (meal == null)
            {
                throw new InvalidOperationException("Meal not found");
            }
            if (meal.User_id != userId)
            {
                throw new InvalidOperationException("You do not have permission to modify this meal");
            }
            return await mealsRepository.UpdateTemplateStatusAsync(id, template);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error updating template status for meal ID {id}: {exception.Message}", exception);
        }
    }

    // Fetch all unique ingredients from a list of meal IDs for a specific user
    public async Task<IEnumerable<IngredientDTO>> GetUniqueIngredientsFromMealsAsync(List<int> mealIds, int userId)
    {
        try
        {
            // Verify all meals belong to the user
            foreach (var mealId in mealIds)
            {
                var meal = await mealsRepository.GetByIdAsync(mealId);
                if (meal == null || meal.User_id != userId)
                {
                    throw new InvalidOperationException("You do not have permission to access all specified meals");
                }
            }

            // Fetch all unique ingredients from the specified meals
            var Ingredients = new List<IngredientDTO>();
            foreach (var mealId in mealIds)
            {
                var packedIngredients = await packedRepository.GetAllByMealIdAsync(mealId);
                foreach (var packedIngredient in packedIngredients)
                {
                    var ingredient = await ingredientRepository.GetByIdAsync(packedIngredient.Ingredient_id);
                    if (ingredient != null && !Ingredients.Any(i => i.Id == ingredient.Id))
                    {
                        Ingredients.Add(ingredient);
                    }
                }
            }
            return Ingredients;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving unique ingredients from meals: {exception.Message}", exception);
        }
    }

    // Helper method to add ingredients to a meal and return a MealWithIngredientsDTO.
    private async Task<MealWithIngredientsDTO> AddIngredientsToMealAsync(MealDTO meal)
    {
        var packedIngredients = await packedRepository.GetAllByMealIdAsync(meal.Id);
        var ingredients = await Task.WhenAll(packedIngredients.Select(p => ingredientRepository.GetByIdAsync(p.Ingredient_id)));
        var mealWithIngredients = new MealWithIngredientsDTO
        {
            Id = meal.Id,
            Name = meal.Name,
            Date = meal.Date,
            Template = meal.Template,
            Image_id = meal.Image_id,
            User_id = meal.User_id,
            Ingredients = ingredients.Where(i => i != null).Select(i => i!).ToList()
        };
        return mealWithIngredients;
    }
}