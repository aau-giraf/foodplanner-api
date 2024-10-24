using Dapper;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

namespace testing
{
    public class IngredientRepositoryTests
    {
    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IEnumerable<Ingredient> expected = [];
        
        //Attempt
        IEnumerable<Ingredient> actual = await ingredientRep.GetAllAsync();
        
        //Verify
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void GetAllAsync_OneIngredientInDatabase_ReturnsOneIngredient()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> actual = await ingredientRep.GetAllAsync();
        
        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        
        //Attempt
        Ingredient actual = await ingredientRep.GetByIdAsync(0);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OneIngredientInDatabase_ReturnsTheSameIngredient()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};
        string expected = "test";
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);
        
        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void UpdateAsync_OneIngredientInDatabase_ReturnsTheUpdatedIngredient()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        Ingredient ingredient = new() { Id = 0, Name = "old test", User_ref = 1, Image_ref = 1};
        Ingredient updatedIngredient = new() { Id = 0, Name = "new test", User_ref = 1, Image_ref = 1};
        string expected = "new test";
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        await ingredientRep.UpdateAsync(updatedIngredient, ingredientId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);
        
        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void DeleteAsync_OneIngredientInDatabase_ReturnsNull()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        await ingredientRep.DeleteAsync(ingredientId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void Z() //The tests are called alphabeticly, so this is called Z to force it to be last
    {
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        Assert.True(true);
    }
    }
}