using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;

namespace Test.Repository;

[Collection("Database collection")]
public class IngredientRepositoryTest(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _fixture = fixture;

    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        IngredientRepository ingredientRep = new(_fixture.GetFactory());
        
        //Attempt
        IEnumerable<Ingredient> actual = await ingredientRep.GetAllAsync();
        
        //Verify
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAllAsync_OneIngredientInDatabase_ReturnsOneIngredient()
    {
        //Setup
        IngredientRepository ingredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        IngredientDTO ingredient = new() { Name = "test", Food_image_id = imageId};
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient, userId);
        IEnumerable<Ingredient> actual = await ingredientRep.GetAllAsync();

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        //Setup
        IngredientRepository ingredientRep = new(_fixture.GetFactory());
        
        //Attempt
        Ingredient actual = await ingredientRep.GetByIdAsync(0);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OneIngredientInDatabase_ReturnsTheSameIngredient()
    {
        //Setup
        IngredientRepository ingredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        IngredientDTO ingredient = new() { Name = "test", Food_image_id = imageId};
        string expected = "test";
        
        //Attempt
        int ingredientId = await ingredientRep.InsertAsync(ingredient, userId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void UpdateAsync_OneIngredientInDatabase_ReturnsTheUpdatedIngredient()
    {
        //Setup
        IngredientRepository ingredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        IngredientDTO ingredient = new() { Name = "old test", Food_image_id = imageId};
        Ingredient updatedIngredient = new() { Id = 0, Name = "new test", User_id = userId, Food_image_id = imageId};
        string expected = "new test";
        
        //Attempt
        int ingredientId = await ingredientRep.InsertAsync(ingredient, userId);
        await ingredientRep.UpdateAsync(updatedIngredient, ingredientId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void DeleteAsync_OneIngredientInDatabase_ReturnsNull()
    {
        //Setup
        IngredientRepository ingredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        IngredientDTO ingredient = new() { Name = "test", Food_image_id = imageId};
        
        //Attempt
        int ingredientId = await ingredientRep.InsertAsync(ingredient, userId);
        await ingredientRep.DeleteAsync(ingredientId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Null(actual);
    }
}