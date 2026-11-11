using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;
using Xunit;

namespace Test.Integration.Services;

public class MealsServiceTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly MealsService service;

    public MealsServiceTests()
    {
        var mealsRepository = new MealsRepository(mockExecutor.Object);
        var packedRepository = new PackedRepository(mockExecutor.Object);
        var ingredientsRepository = new IngredientsRepository(mockExecutor.Object);

        service = new MealsService(mealsRepository, packedRepository, ingredientsRepository);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllMealsAsync_ShouldReturnMealsWithIngredientsInOrder()
    {
        // Arrange
        var meal = CreateMeal(1, "Meal With Ingredients", 12345);
        var otherMeal = CreateMeal(2, "Meal Without Ingredients", 67890);
        var firstIngredient = CreateIngredient(10, "First Ingredient", 12345);
        var secondIngredient = CreateIngredient(20, "Second Ingredient", 12345);

        mockExecutor.Setup(e => e.QueryAsync<MealDTO>(
                "SELECT * FROM meals", It.IsAny<object?>()))
            .ReturnsAsync(new[] { meal, otherMeal });

        mockExecutor.Setup(e => e.QueryAsync<PackedDTO>(
                It.Is<string>(s => s.Contains("FROM packed") && !s.Contains("WHERE")),
                It.IsAny<object?>()))
            .ReturnsAsync(new[]
            {
                CreatePacked(meal.Id, secondIngredient.Id, 2),
                CreatePacked(meal.Id, firstIngredient.Id, 1)
            });

        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
                "SELECT * FROM ingredients", It.IsAny<object?>()))
            .ReturnsAsync(new[] { firstIngredient, secondIngredient });

        // Act
        var result = (await service.GetAllMealsAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        var actualMeal = Assert.Single(result.Where(m => m.Id == meal.Id));
        Assert.Equal(meal.Name, actualMeal.Name);
        Assert.Equal(meal.User_id, actualMeal.User_id);
        Assert.Equal(new[] { firstIngredient.Id, secondIngredient.Id },
            actualMeal.Ingredients.Select(i => i.Id).ToArray());
        Assert.Empty(Assert.Single(result.Where(m => m.Id == otherMeal.Id)).Ingredients);

        mockExecutor.Verify(e => e.QueryAsync<MealDTO>(
            "SELECT * FROM meals", It.IsAny<object?>()), Times.Once);
        mockExecutor.Verify(e => e.QueryAsync<PackedDTO>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Once);
        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(
            "SELECT * FROM ingredients", It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllMealsByUserAndDateAsync_ShouldReturnMatchingMealsWithIngredients()
    {
        // Arrange
        const int userId = 12345;
        const string date = "2026-11-02";
        var meal = CreateMeal(3, "Matching Meal", userId, date);
        var ingredient = CreateIngredient(30, "Matching Ingredient", userId);

        mockExecutor.Setup(e => e.QueryAsync<MealDTO>(
                "SELECT * FROM meals WHERE user_id = @UserId AND date = @Date",
                It.Is<object?>(p => HasParameter(p, "UserId", userId) && HasParameter(p, "Date", date))))
            .ReturnsAsync(new[] { meal });
        SetupPackedForMeal(meal.Id, CreatePacked(meal.Id, ingredient.Id, 1));
        SetupIngredientById(ingredient);

        // Act
        var result = (await service.GetAllMealsByUserAndDateAsync(userId, date)).ToList();

        // Assert
        var actualMeal = Assert.Single(result);
        Assert.Equal(meal.Id, actualMeal.Id);
        Assert.Equal(userId, actualMeal.User_id);
        Assert.Equal(date, actualMeal.Date);
        Assert.Equal(ingredient.Id, Assert.Single(actualMeal.Ingredients).Id);
        mockExecutor.Verify(e => e.QueryAsync<MealDTO>(
            "SELECT * FROM meals WHERE user_id = @UserId AND date = @Date",
            It.Is<object?>(p => HasParameter(p, "UserId", userId) && HasParameter(p, "Date", date))),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetMealByIdAsync_ShouldReturnMealWithIngredients()
    {
        // Arrange
        var meal = CreateMeal(4, "Specific Meal", 12345);
        var ingredient = CreateIngredient(40, "Specific Ingredient", 12345);
        SetupMealById(meal);
        SetupPackedForMeal(meal.Id, CreatePacked(meal.Id, ingredient.Id, 1));
        SetupIngredientById(ingredient);

        // Act
        var result = await service.GetMealByIdAsync(meal.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meal.Id, result.Id);
        Assert.Equal(meal.Name, result.Name);
        Assert.Equal(ingredient.Id, Assert.Single(result.Ingredients).Id);
        mockExecutor.Verify(e => e.QuerySingleOrDefaultAsync<MealDTO>(
            "SELECT * FROM meals WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", meal.Id))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetMealByIdAsync_ShouldReturnNullForMissingMeal()
    {
        // Arrange
        const int missingId = -1;
        SetupMissingMeal(missingId);

        // Act
        var result = await service.GetMealByIdAsync(missingId);

        // Assert
        Assert.Null(result);
        mockExecutor.Verify(e => e.QueryAsync<PackedDTO>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateMealAsync_ShouldReturnGeneratedId()
    {
        // Arrange
        const int userId = 12345;
        const int generatedId = 77;
        var meal = new MealCreateDTO
        {
            Name = "New Meal",
            Date = "2026-11-02",
            Template = false,
            Image_id = null
        };

        mockExecutor.Setup(e => e.QuerySingleAsync<int>(
                "INSERT INTO meals (name, user_id, image_id, date, template) VALUES (@Name, @UserId, @ImageId, @Date, @Template) RETURNING id",
                It.Is<object?>(p =>
                    HasParameter(p, "Name", meal.Name) &&
                    HasParameter(p, "UserId", userId) &&
                    HasParameter(p, "ImageId", DBNull.Value) &&
                    HasParameter(p, "Date", meal.Date) &&
                    HasParameter(p, "Template", meal.Template))))
            .ReturnsAsync(generatedId);

        // Act
        int result = await service.CreateMealAsync(meal, userId);

        // Assert
        Assert.Equal(generatedId, result);
        mockExecutor.Verify(e => e.QuerySingleAsync<int>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateMealAsync_ShouldReturnAffectedRows()
    {
        // Arrange
        var updated = CreateMeal(5, "Updated Meal", 67890, "2026-11-03", true);

        mockExecutor.Setup(e => e.ExecuteAsync(
                "UPDATE meals SET name = @Name, user_id = @UserId, image_id = @ImageId, date = @Date, template = @Template WHERE id = @Id",
                It.Is<object?>(p =>
                    HasParameter(p, "Id", updated.Id) &&
                    HasParameter(p, "Name", updated.Name) &&
                    HasParameter(p, "UserId", updated.User_id) &&
                    HasParameter(p, "ImageId", DBNull.Value) &&
                    HasParameter(p, "Date", updated.Date) &&
                    HasParameter(p, "Template", updated.Template))))
            .ReturnsAsync(1);

        // Act
        int result = await service.UpdateMealAsync(updated);

        // Assert
        Assert.Equal(1, result);
        mockExecutor.Verify(e => e.ExecuteAsync(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteMealAsync_ShouldReturnAffectedRows()
    {
        // Arrange
        const int mealId = 6;
        mockExecutor.Setup(e => e.ExecuteAsync(
                "DELETE FROM meals WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", mealId))))
            .ReturnsAsync(1);

        // Act
        int result = await service.DeleteMealAsync(mealId);

        // Assert
        Assert.Equal(1, result);
        mockExecutor.Verify(e => e.ExecuteAsync(
            "DELETE FROM meals WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", mealId))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllTemplatesByUserAsync_ShouldReturnUserTemplatesWithIngredients()
    {
        // Arrange
        const int userId = 12345;
        var template = CreateMeal(7, "User Template", userId, template: true);
        var ingredient = CreateIngredient(70, "Template Ingredient", userId);

        mockExecutor.Setup(e => e.QueryAsync<MealDTO>(
                "SELECT * FROM meals WHERE template = true AND user_id = @UserId",
                It.Is<object?>(p => HasParameter(p, "UserId", userId))))
            .ReturnsAsync(new[] { template });
        SetupPackedForMeal(template.Id, CreatePacked(template.Id, ingredient.Id, 1));
        SetupIngredientById(ingredient);

        // Act
        var result = (await service.GetAllTemplatesByUserAsync(userId)).ToList();

        // Assert
        var actual = Assert.Single(result);
        Assert.Equal(template.Id, actual.Id);
        Assert.True(actual.Template);
        Assert.Equal(userId, actual.User_id);
        Assert.Equal(ingredient.Id, Assert.Single(actual.Ingredients).Id);
        mockExecutor.Verify(e => e.QueryAsync<MealDTO>(
            "SELECT * FROM meals WHERE template = true AND user_id = @UserId",
            It.Is<object?>(p => HasParameter(p, "UserId", userId))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateTemplateStatusAsync_ShouldUpdateOwnedMeal()
    {
        // Arrange
        const int userId = 12345;
        var meal = CreateMeal(8, "Owned Meal", userId);
        SetupMealById(meal);
        mockExecutor.Setup(e => e.ExecuteAsync(
                "UPDATE meals SET template = @template WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", meal.Id) && HasParameter(p, "Template", true))))
            .ReturnsAsync(1);

        // Act
        int result = await service.UpdateTemplateStatusAsync(meal.Id, true, userId);

        // Assert
        Assert.Equal(1, result);
        mockExecutor.Verify(e => e.QuerySingleOrDefaultAsync<MealDTO>(
            "SELECT * FROM meals WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", meal.Id))), Times.Once);
        mockExecutor.Verify(e => e.ExecuteAsync(
            "UPDATE meals SET template = @template WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", meal.Id) && HasParameter(p, "Template", true))),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateTemplateStatusAsync_ShouldRejectDifferentUser()
    {
        // Arrange
        var meal = CreateMeal(9, "Someone Else's Meal", 12345);
        SetupMealById(meal);

        // Act
        var error = await Assert.ThrowsAsync<Exception>(
            () => service.UpdateTemplateStatusAsync(meal.Id, true, 67890));

        // Assert
        Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("You do not have permission", error.Message);
        mockExecutor.Verify(e => e.ExecuteAsync(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateTemplateStatusAsync_ShouldRejectMissingMeal()
    {
        // Arrange
        const int missingId = -1;
        SetupMissingMeal(missingId);

        // Act
        var error = await Assert.ThrowsAsync<Exception>(
            () => service.UpdateTemplateStatusAsync(missingId, true, 12345));

        // Assert
        Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("Meal not found", error.Message);
        mockExecutor.Verify(e => e.ExecuteAsync(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUniqueIngredientsFromMealsAsync_ShouldReturnDistinctIngredients()
    {
        // Arrange
        const int userId = 12345;
        var firstMeal = CreateMeal(11, "First Meal", userId);
        var secondMeal = CreateMeal(12, "Second Meal", userId);
        var sharedIngredient = CreateIngredient(101, "Shared Ingredient", userId);
        var otherIngredient = CreateIngredient(102, "Other Ingredient", userId);

        SetupMealById(firstMeal);
        SetupMealById(secondMeal);
        SetupPackedForMeal(firstMeal.Id, CreatePacked(firstMeal.Id, sharedIngredient.Id, 1));
        SetupPackedForMeal(secondMeal.Id,
            CreatePacked(secondMeal.Id, sharedIngredient.Id, 1),
            CreatePacked(secondMeal.Id, otherIngredient.Id, 2));
        SetupIngredientById(sharedIngredient);
        SetupIngredientById(otherIngredient);

        // Act
        var result = (await service.GetUniqueIngredientsFromMealsAsync(
            new List<int> { firstMeal.Id, secondMeal.Id }, userId)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Id == sharedIngredient.Id);
        Assert.Contains(result, i => i.Id == otherIngredient.Id);
        Assert.Equal(2, result.Select(i => i.Id).Distinct().Count());
        mockExecutor.Verify(e => e.QueryAsync<PackedDTO>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Exactly(2));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUniqueIngredientsFromMealsAsync_ShouldRejectOtherUsersMeals()
    {
        // Arrange
        const int userId = 12345;
        var owned = CreateMeal(13, "Owned Meal", userId);
        var other = CreateMeal(14, "Other User Meal", 67890);
        SetupMealById(owned);
        SetupMealById(other);

        // Act
        var error = await Assert.ThrowsAsync<Exception>(
            () => service.GetUniqueIngredientsFromMealsAsync(
                new List<int> { owned.Id, other.Id }, userId));

        // Assert
        Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("You do not have permission", error.Message);
        mockExecutor.Verify(e => e.QueryAsync<PackedDTO>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUniqueIngredientsFromMealsAsync_ShouldRejectMissingMeal()
    {
        // Arrange
        const int missingId = -1;
        SetupMissingMeal(missingId);

        // Act
        var error = await Assert.ThrowsAsync<Exception>(
            () => service.GetUniqueIngredientsFromMealsAsync(new List<int> { missingId }, 12345));

        // Assert
        Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("You do not have permission", error.Message);
        mockExecutor.Verify(e => e.QueryAsync<PackedDTO>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    // helper methods
    private void SetupMealById(MealDTO meal)
    {
        mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<MealDTO>(
                "SELECT * FROM meals WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", meal.Id))))
            .ReturnsAsync(meal);
    }

    private void SetupMissingMeal(int id)
    {
        mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<MealDTO>(
                "SELECT * FROM meals WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", id))))
            .ReturnsAsync((MealDTO?)null);
    }

    private void SetupIngredientById(IngredientDTO ingredient)
    {
        mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
                "SELECT * FROM ingredients WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", ingredient.Id))))
            .ReturnsAsync(ingredient);
    }

    private void SetupPackedForMeal(int mealId, params PackedDTO[] packed)
    {
        mockExecutor.Setup(e => e.QueryAsync<PackedDTO>(
                It.Is<string>(sql => sql.Contains("FROM packed") && sql.Contains("WHERE meal_id")),
                It.Is<object?>(p => HasParameter(p, "Id", mealId) || HasParameter(p, "MealId", mealId))))
            .ReturnsAsync(packed);
    }

    private static MealDTO CreateMeal(
        int id, string name, int userId, string date = "2026-11-02", bool template = false)
    {
        return new MealDTO
        {
            Id = id,
            Name = name,
            User_id = userId,
            Date = date,
            Template = template,
            Image_id = null
        };
    }

    private static IngredientDTO CreateIngredient(int id, string name, int userId)
    {
        return new IngredientDTO
        {
            Id = id,
            Name = name,
            User_id = userId,
            Image_id = null
        };
    }

    private static PackedDTO CreatePacked(int mealId, int ingredientId, int orderNo)
    {
        return new PackedDTO
        {
            Meal_id = mealId,
            Ingredient_id = ingredientId,
            order_no = orderNo
        };
    }

    private static bool HasParameter(object? parameters, string name, object? expected)
    {
        if (parameters is null)
            return false;

        var property = parameters.GetType().GetProperty(name);
        return property is not null && Equals(property.GetValue(parameters), expected);
    }
}
