using Dapper;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerDataAccessSql;

namespace testing;

/**
* These are technicly integration tests, since the database has not been mocked using Xunits Fixture.
* It is therefor important to check that the database SW5-10 is empty before running the tests.
*/
[Collection("Sequential")] // Indicates that the tests in this collection should run sequentially to avoid issues with shared state.
public class PackedIngredientRepositoryTests 
{
    private static readonly PackedIngredientRepository packedIngredientRep = new(DatabaseConnection.GetConnection());
    private static readonly MealRepository mealRep = new(DatabaseConnection.GetConnection());
    private static readonly IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());

    [Fact] // Marks the method as a test method.
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database to ensure it is empty.
        IEnumerable<PackedIngredient> expected = []; // The expected result is an empty list.
        
        // Attempt
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllAsync(); // Returns all PackedIngredients from the database.
        
        // Verify
        Assert.Empty(actual); // Asserts that the actual list is empty.
        Assert.Equal(expected, actual); // Asserts that the expected result matches the actual result.
    }

    [Fact]
    public async void GetAllAsync_OnePackedIngredientInDatabase_ReturnsOnePackedIngredient()
    {
        // Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        
        // Creates a test meal and ingredient.
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test" };
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1 };
        
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
        PackedIngredient packedIngredient = new() { Id = 3, Meal_ref = mealId, Ingredient_ref = ingredientId };
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllAsync(); // Returns all packed ingredients from the database.

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify
        Assert.Single(actual); // Asserts that there is exactly one packed ingredient in the database.
    }

    [Fact] // Marks the method as a test method.
    public async void GetAllByMealIdAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database to ensure it is empty.
        IEnumerable<PackedIngredient> expected = []; // The expected result is an empty list.
        
        // Attempt
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllByMealIdAsync(0); // Returns all PackedIngredients by meal id from the database.
        
        // Verify
        Assert.Empty(actual); // Asserts that the actual list is empty.
        Assert.Equal(expected, actual); // Asserts that the expected result matches the actual result.
    }

    [Fact]
    public async void GetAllByMealIdAsync_TwoPackedIngredientInDatabase_ReturnsOnePackedIngredient()
    {
        // Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        
        // Creates test meals and ingredient.
        Meal meal1 = new() { Id = 1, Title = "test1", User_ref = 1, Image_ref = 1, Date = "test" };
        Meal meal2 = new() { Id = 2, Title = "test2", User_ref = 1, Image_ref = 1, Date = "test" };
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1 };
        
        // Inserts the meals and ingredient into the database.
        await mealRep.InsertAsync(meal1);
        await mealRep.InsertAsync(meal2);
        await ingredientRep.InsertAsync(ingredient);

        // Creates the PackedIngredients to insert into the database.
        PackedIngredient packedIngredient1 = new() { Id = 1, Meal_ref = 1, Ingredient_ref = 1 };
        PackedIngredient packedIngredient2 = new() { Id = 2, Meal_ref = 2, Ingredient_ref = 1 };
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient1); // Inserts the first packed ingredient into the database.
        await packedIngredientRep.InsertAsync(packedIngredient2); // Inserts the second packed ingredient into the database.
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllByMealIdAsync(1); // Returns all packed ingredients with meal id 1 from the database.

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify
        Assert.Single(actual); // Asserts that only one packed ingredient was returned.
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        // Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        
        // Attempt
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(0); // Attempts to retrieve a packed ingredient by ID.
        
        // Verify
        Assert.Null(actual); // Asserts that the actual result is null (no packed ingredient found).
    }

    [Fact]
    public async void GetByIdAsync_OnePackedIngredientInDatabase_ReturnsTheSamePackedIngredient()
    {
        // Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.
        
        // Creates a test meal and ingredient.
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test" };
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1 };
        
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
        PackedIngredient packedIngredient = new() { Id = 3, Meal_ref = mealId, Ingredient_ref = ingredientId };
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync(); // Returns all packed ingredients from the database.
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id; // Gets the ID of the inserted packed ingredient.
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId); // Returns the packed ingredient by ID.
        int expected = packedIngredientId; // Sets the expected value to the packed ingredient ID.

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify
        Assert.Equal(expected, actual.Id); // Asserts that the actual ID matches the expected ID.
    }

    [Fact]
    public async void UpdateAsync_OneMealInDatabase_ReturnsTheUpdatedMeal()
    {
        // Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.

        // Creates two test meals and one ingredient.
        Meal meal = new() { Id = 1, Title = "old test", User_ref = 1, Image_ref = 1, Date = "test" };
        Meal mealnew = new() { Id = 2, Title = "new test", User_ref = 1, Image_ref = 1, Date = "test" };               
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1 };
        
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
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId };
        PackedIngredient updatedPackedIngredient = new() { Id = 2, Meal_ref = mealNewID, Ingredient_ref = ingredientId }; // Updates the meal reference.
        int expected = updatedPackedIngredient.Meal_ref; // Sets the expected value for the meal reference.
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync(); // Returns all packed ingredients.
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id; // Gets the ID of the inserted packed ingredient.
        await packedIngredientRep.UpdateAsync(updatedPackedIngredient, packedIngredientId); // Updates the packed ingredient in the database.
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId); // Returns the updated packed ingredient.
        await packedIngredientRep.DeleteAsync(packedIngredientId); // Cleans up by deleting the packed ingredient.

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify
        Assert.Equal(expected, actual.Meal_ref); // Asserts that the updated meal reference matches the expected value.
    }

    [Fact]
    public async void DeleteAsync_OneMealInDatabase_ReturnsNull()
    {
        // Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients"); // Clears the database.

        // Creates a test meal and ingredient.
        Meal meal = new() { Id = 1, Title = "test", User_ref = 1, Image_ref = 1, Date = "test" };        
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1 };
        
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
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId };
        
        // Attempt
        await packedIngredientRep.InsertAsync(packedIngredient); // Inserts the packed ingredient into the database.
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync(); // Returns all packed ingredients.
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id; // Gets the ID of the inserted packed ingredient.
        await packedIngredientRep.DeleteAsync(packedIngredientId); // Deletes the packed ingredient from the database.
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId); // Attempts to retrieve the deleted packed ingredient.

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify
        Assert.Null(actual); // Asserts that the actual result is null (the packed ingredient has been deleted).
    }
}
