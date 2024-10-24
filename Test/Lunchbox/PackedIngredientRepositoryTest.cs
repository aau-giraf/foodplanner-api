using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using Xunit;

namespace testing;
[Collection("Sequential")] // Indicates that the tests in this collection should run sequentially to avoid issues with shared state.
public class PackedIngredientRepositoryTests 
{
    [Fact] // Marks the method as a test method.
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database to ensure it is empty.
        PackedIngredientRepository packedIngredientRepo = new PackedIngredientRepository(DatabaseConnection.GetConnection()); // Creates an instance of the repository.
        IEnumerable<PackedIngredient> expected = Enumerable.Empty<PackedIngredient>(); // The expected result is an empty list.
        
        // Attempt
        IEnumerable<PackedIngredient> actual = await packedIngredientRepo.GetAllAsync(); // Returns all PackedIngredients from the database.
        
        // Verify
        Assert.Empty(actual); // Asserts that the actual list is empty.
        Assert.Equal(expected, actual); // Asserts that the expected result matches the actual result.
    }

    [Fact]
    public async void GetAllAsync_OnePackedIngredientInDatabase_ReturnsOnePackedIngredient()
    {
        // Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        PackedIngredientRepository packedIngredientRepo = new PackedIngredientRepository(DatabaseConnection.GetConnection()); // Creates the repository instance.
        
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection()); // Creates ingredient repository.
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection()); // Creates meal repository.
        
        // Creates a test meal and ingredient.
        Meal meal = new Meal { Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test" };
        Ingredient ingredient = new Ingredient { Id = 1, Name = "test", User_ref = "alex", Image_ref = "test" };
        
        // Inserts the meal and ingredient into the database.
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Returns all meals and ingredients.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        
        // Gets the IDs of the inserted meal and ingredient.
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;

        // Creates a PackedIngredient to insert into the database.
        PackedIngredient packedIngredient = new PackedIngredient { Id = 3, Meal_ref = mealId, Ingredient_ref = ingredientId };
        
        // Attempt
        await packedIngredientRepo.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> actual = await packedIngredientRepo.GetAllAsync(); // Returns all packed ingredients from the database.
        
        // Verify
        Assert.Single(actual); // Asserts that there is exactly one packed ingredient in the database.
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        // Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        PackedIngredientRepository packedIngredientRepo = new PackedIngredientRepository(DatabaseConnection.GetConnection()); // Creates the repository instance.
        
        // Attempt
        PackedIngredient actual = await packedIngredientRepo.GetByIdAsync(0); // Attempts to retrieve a packed ingredient by ID.
        
        // Verify
        Assert.Null(actual); // Asserts that the actual result is null (no packed ingredient found).
    }

    [Fact]
    public async void GetByIdAsync_OnePackedIngredientInDatabase_ReturnsTheSamePackedIngredient()
    {
        // Setup
        PackedIngredientRepository packedIngredientRepo = new PackedIngredientRepository(DatabaseConnection.GetConnection()); // Creates the repository instance.
        
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection()); // Creates ingredient repository.
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection()); // Creates meal repository.
        
        // Creates a test meal and ingredient.
        Meal meal = new Meal { Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test" };
        Ingredient ingredient = new Ingredient { Id = 1, Name = "test", User_ref = "alex", Image_ref = "test" };
        
        // Inserts the meal and ingredient into the database.
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Returns all meals and ingredients.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        
        // Gets the IDs of the inserted meal and ingredient.
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;

        // Creates a PackedIngredient to insert into the database.
        PackedIngredient packedIngredient = new PackedIngredient { Id = 3, Meal_ref = mealId, Ingredient_ref = ingredientId };
        
        // Attempt
        await packedIngredientRepo.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRepo.GetAllAsync(); // Returns all packed ingredients from the database.
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id; // Gets the ID of the inserted packed ingredient.
        PackedIngredient actual = await packedIngredientRepo.GetByIdAsync(packedIngredientId); // Returns the packed ingredient by ID.
        int expected = packedIngredientId; // Sets the expected value to the packed ingredient ID.

        // Verify
        Assert.Equal(expected, actual.Id); // Asserts that the actual ID matches the expected ID.
    }

    [Fact]
    public async void UpdateAsync_OneMealInDatabase_ReturnsTheUpdatedMeal()
    {
        // Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        PackedIngredientRepository packedIngredientRep = new PackedIngredientRepository(DatabaseConnection.GetConnection()); // Creates the repository instance.

        // Setup meal and ingredient
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection()); // Creates meal repository.
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection()); // Creates ingredient repository.

        // Creates two test meals and one ingredient.
        Meal meal = new Meal { Id = 1, Title = "test", User_ref = "old", Image_ref = "test", Date = "test" };
        Meal mealnew = new Meal { Id = 2, Title = "test", User_ref = "new", Image_ref = "test", Date = "test" };               
        Ingredient ingredient = new Ingredient { Id = 1, Name = "test", User_ref = "alex", Image_ref = "test" };
        
        // Inserts the meals and ingredient into the database.
        await mealRep.InsertAsync(meal);
        await mealRep.InsertAsync(mealnew);
        await ingredientRep.InsertAsync(ingredient);

        // Returns all meals and ingredients.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        
        // Gets the IDs of the inserted meals and ingredient.
        int mealId = allMeals.FirstOrDefault().Id;
        int mealNewID = allMeals.Skip(1).FirstOrDefault().Id; // Gets the ID of the second meal.
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        // Creates a PackedIngredient to insert into the database.
        PackedIngredient packedIngredient = new PackedIngredient { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId };
        PackedIngredient updatedPackedIngredient = new PackedIngredient { Id = 2, Meal_ref = mealNewID, Ingredient_ref = ingredientId }; // Updates the meal reference.
        int expected = updatedPackedIngredient.Meal_ref; // Sets the expected value for the meal reference.
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync(); // Returns all packed ingredients.
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id; // Gets the ID of the inserted packed ingredient.
        await packedIngredientRep.UpdateAsync(updatedPackedIngredient, packedIngredientId); // Updates the packed ingredient in the database.
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId); // Returns the updated packed ingredient.
        await packedIngredientRep.DeleteAsync(packedIngredientId); // Cleans up by deleting the packed ingredient.

        // Verify
        Assert.Equal(expected, actual.Meal_ref); // Asserts that the updated meal reference matches the expected value.
    }

    [Fact]
    public async void DeleteAsync_OneMealInDatabase_ReturnsNull()
    {
        // Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        PackedIngredientRepository packedIngredientRep = new PackedIngredientRepository(DatabaseConnection.GetConnection()); // Creates the repository instance.

        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection()); // Creates meal repository.
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection()); // Creates ingredient repository.

        // Creates a test meal and ingredient.
        Meal meal = new Meal { Id = 1, Title = "test", User_ref = "old", Image_ref = "test", Date = "test" };        
        Ingredient ingredient = new Ingredient { Id = 1, Name = "test", User_ref = "alex", Image_ref = "test" };
        
        // Inserts the meal and ingredient into the database.
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Returns all meals and ingredients.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        
        // Gets the IDs of the inserted meal and ingredient.
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        // Creates a PackedIngredient to insert into the database.
        PackedIngredient packedIngredient = new PackedIngredient { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId };
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync(); // Returns all packed ingredients.
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id; // Gets the ID of the inserted packed ingredient.
        await packedIngredientRep.DeleteAsync(packedIngredientId); // Deletes the packed ingredient from the database.
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId); // Attempts to retrieve the deleted packed ingredient.
        
        // Verify
        Assert.Null(actual); // Asserts that the actual result is null (the packed ingredient has been deleted).
    }
}
