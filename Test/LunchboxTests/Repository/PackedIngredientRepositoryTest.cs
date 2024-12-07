using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;

namespace Test.Repository;

[Collection("Database collection")]
public class PackedIngredientRepositoryTest(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _fixture = fixture;

    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        
        // Attempt
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllAsync();
        
        // Verify
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAllAsync_OnePackedIngredientInDatabase_ReturnsOnePackedIngredient()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        int mealId = await _fixture.CreateTestMeal(userId, imageId);
        int ingredientId = await _fixture.CreateTestIngredient(userId, imageId);
        
        // Attempt
        await packedIngredientRep.InsertAsync(mealId, ingredientId);
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllAsync();

        //Clean Up
        _fixture.ResetDatabase();

        // Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetAllByMealIdAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        
        // Attempt
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllByMealIdAsync(0);
        
        // Verify
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAllByMealIdAsync_TwoPackedIngredientInDatabase_ReturnsOnePackedIngredient()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        int mealId = await _fixture.CreateTestMeal(userId, imageId);
        int mealId2 = await _fixture.CreateTestMeal(userId, imageId);
        int ingredientId = await _fixture.CreateTestIngredient(userId, imageId);
        
        // Attempt
        await packedIngredientRep.InsertAsync(mealId, ingredientId);
        await packedIngredientRep.InsertAsync(mealId2, ingredientId);
        IEnumerable<PackedIngredient> actual = await packedIngredientRep.GetAllByMealIdAsync(mealId);

        //Clean Up
        _fixture.ResetDatabase();

        // Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        
        // Attempt
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(0);
        
        // Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OnePackedIngredientInDatabase_ReturnsTheSamePackedIngredient()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        int mealId = await _fixture.CreateTestMeal(userId, imageId);
        int ingredientId = await _fixture.CreateTestIngredient(userId, imageId);
        
        // Attempt
        int packedIngredientId = await packedIngredientRep.InsertAsync(mealId, ingredientId);
        int expected = packedIngredientId;
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId);

        //Clean Up
        _fixture.ResetDatabase();

        // Verify
        Assert.Equal(expected, actual.Id);
    }

    [Fact]
    public async void UpdateAsync_OneMealInDatabase_ReturnsTheUpdatedMeal()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        int mealId = await _fixture.CreateTestMeal(userId, imageId);
        int expected = await _fixture.CreateTestMeal(userId, imageId);
        int ingredientId = await _fixture.CreateTestIngredient(userId, imageId);
        PackedIngredient updatedPackedIngredient = new() { Id = 0, Meal_id = expected, Ingredient_id = ingredientId, order_number = 0};
        
        // Attempt
        int packedIngredientId = await packedIngredientRep.InsertAsync(mealId, ingredientId);
        await packedIngredientRep.UpdateAsync(updatedPackedIngredient, packedIngredientId);
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId);

        //Clean Up
        _fixture.ResetDatabase();

        // Verify
        Assert.Equal(expected, actual.Meal_id);
    }

    [Fact]
    public async void DeleteAsync_OneMealInDatabase_ReturnsNull()
    {
        // Setup
        PackedIngredientRepository packedIngredientRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        int mealId = await _fixture.CreateTestMeal(userId, imageId);
        int ingredientId = await _fixture.CreateTestIngredient(userId, imageId);
        
        // Attempt
        int packedIngredientId = await packedIngredientRep.InsertAsync(mealId, ingredientId);
        await packedIngredientRep.DeleteAsync(packedIngredientId);
        PackedIngredient actual = await packedIngredientRep.GetByIdAsync(packedIngredientId);

        //Clean Up
        _fixture.ResetDatabase();

        // Verify
        Assert.Null(actual);
    }
}