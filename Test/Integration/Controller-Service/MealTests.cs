
using FoodplannerApi.Controller;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authentication;

namespace Test.Integration.Controllers;

public class MealsTests
{
    private readonly Mock<IMealsRepository> mockRepository = new();
    private readonly Mock<IPackedRepository> mockPackedRepository = new();
    private readonly Mock<IIngredientsRepository> mockIngredientsRepository = new();
    private readonly Mock<IAuthService> mockAuthService = new();
    private readonly MealsController controller;

    public MealsTests()
    {
        var service = new MealsService(
            mockRepository.Object,
            mockPackedRepository.Object,
            mockIngredientsRepository.Object);

        controller = new MealsController(service, mockAuthService.Object);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllMeals_ShouldReturnCorrectMeals()
    {
        // Arrange
        int mealId1 = 42;
        int mealId2 = 32;
        var expected1 = CreateMeal(mealId1, "Test Meal1", 12345);
        var expected2 = CreateMeal(mealId2, "Test Meal2", 12345);

        var expectedMeals = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedMeals);

        mockPackedRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        mockIngredientsRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var result = await controller.GetAllMeals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();

        Assert.Equal(2, meals.Count);
        Assert.Equal(expected1.Id, meals[0].Id);
        Assert.Equal(expected1.Name, meals[0].Name);
        Assert.Equal(expected2.Id, meals[1].Id);
        Assert.Equal(expected2.Name, meals[1].Name);

        Assert.All(meals, meal => Assert.Empty(meal.Ingredients));

        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllAsync(), Times.Once);
        mockIngredientsRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByUser_ShouldGetAllMealsByUser()
    {
        // Arrange
        int userId = 12345;
        string token = "Bearer test-token";
        string date = "2026-11-16";
        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());
        int mealId1 = 42;
        int mealId2 = 32;
        var expected1 = CreateMeal(mealId1, "Test Meal1", userId);
        var expected2 = CreateMeal(mealId2, "Test Meal2", userId);

        var expectedMeals = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllByUserAndDateAsync(userId, date))
            .ReturnsAsync(expectedMeals);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId1))
            .ReturnsAsync([]);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId2))
            .ReturnsAsync([]);

        //Act
        var result = await controller.GetAllByUser(token, date);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();

        Assert.Equal(2, meals.Count);
        Assert.Equal(expected1.Id, meals[0].Id);
        Assert.Equal(expected1.Name, meals[0].Name);
        Assert.Equal(expected2.Id, meals[1].Id);
        Assert.Equal(expected2.Name, meals[1].Name);

        Assert.All(meals, meal =>
        {
            Assert.Equal(userId, meal.User_id);
            Assert.Equal(date, meal.Date);
            Assert.Empty(meal.Ingredients);
        });

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(token), Times.Once);
        mockRepository.Verify(r => r.GetAllByUserAndDateAsync(userId, date), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllByMealIdAsync(mealId1), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllByMealIdAsync(mealId2), Times.Once);
        mockIngredientsRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllMealsTeacher_ShouldGetAllMealsFromSpecificUser()
    {
        // Arrange
        int userId = 12345;
        string date = "2026-11-16";
        int mealId1 = 42;
        int mealId2 = 32;
        var expected1 = CreateMeal(mealId1, "Test Meal1", userId);
        var expected2 = CreateMeal(mealId2, "Test Meal2", userId);

        var expectedMeals = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllByUserAndDateAsync(userId, date))
            .ReturnsAsync(expectedMeals);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId1))
            .ReturnsAsync([]);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId2))
            .ReturnsAsync([]);

        //Act
        var result = await controller.TeacherGetUserMeals(date, userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();

        Assert.Equal(2, meals.Count);
        Assert.Equal(expected1.Id, meals[0].Id);
        Assert.Equal(expected1.Name, meals[0].Name);
        Assert.Equal(expected2.Id, meals[1].Id);
        Assert.Equal(expected2.Name, meals[1].Name);

        Assert.All(meals, meal =>
        {
            Assert.Equal(userId, meal.User_id);
            Assert.Equal(date, meal.Date);
            Assert.Empty(meal.Ingredients);
        });

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(It.IsAny<string>()), Times.Never);
        mockRepository.Verify(r => r.GetAllByUserAndDateAsync(userId, date), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllByMealIdAsync(mealId1), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllByMealIdAsync(mealId2), Times.Once);
        mockIngredientsRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetMeal_ShouldGetMeal()
    {
        // Arrange
        int userId = 12345;
        int mealId = 42;
        var expected = CreateMeal(mealId, "Test Meal1", userId);

        mockRepository.Setup(r => r.GetByIdAsync(mealId))
            .ReturnsAsync(expected);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId))
            .ReturnsAsync([]);

        //Act
        var result = await controller.GetMeal(mealId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meal = Assert.IsType<MealWithIngredientsDTO>(okResult.Value);

        Assert.Equal(expected.Id, meal.Id);
        Assert.Equal(expected.Name, meal.Name);
        Assert.Equal(expected.User_id, meal.User_id);
        Assert.Equal(expected.Date, meal.Date);
        Assert.Empty(meal.Ingredients);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateAsync_ShouldCreateMeal()
    {
        //Arrange
        int userId = 12345;
        string token = "Bearer test-token";
        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        int generatedId = 42;
        string date = "2026-11-16";

        var mealToCreate = new MealCreateDTO
        {
            Name = "Test Meal",
            Date = date,
            Template = false,
            Image_id = null
        };

        var expected = CreateMeal(generatedId, "Test Meal", userId);

        mockRepository.Setup(r => r.InsertAsync(mealToCreate, userId))
            .ReturnsAsync(generatedId);
        mockRepository.Setup(r => r.GetByIdAsync(generatedId))
            .ReturnsAsync(expected);
        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(generatedId))
            .ReturnsAsync([]);

        //Act
        var result = await controller.Create(token,mealToCreate);
        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var meal = Assert.IsType<MealWithIngredientsDTO>(createdResult.Value);

        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(generatedId, meal.Id);
        Assert.Equal(mealToCreate.Name, meal.Name);
        Assert.Equal(userId, meal.User_id);
        Assert.Equal(date, meal.Date);
        Assert.Equal(mealToCreate.Template, meal.Template);
        Assert.Equal(mealToCreate.Image_id, meal.Image_id);
        Assert.Empty(meal.Ingredients);

        Assert.Equal(nameof(MealsController.GetMeal), createdResult.ActionName);
        Assert.Equal(generatedId, createdResult.RouteValues!["id"]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_ShouldUpdateMeal()
    {
        //Arrange
        int userId = 12345;
        string token = "Bearer test-token";
        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());
        
        int mealId = 42;
        var updated = CreateMeal(mealId, "Updated Meal", userId);

        mockRepository.Setup(r => r.UpdateAsync(updated))
            .ReturnsAsync(1);
        mockRepository.Setup(r => r.GetByIdAsync(mealId))
            .ReturnsAsync(updated);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId))
            .ReturnsAsync([]);

        //Act
        var result = await controller.Update(token, updated);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meal = Assert.IsType<MealWithIngredientsDTO>(okResult.Value);

        Assert.Equal(updated.Id, meal.Id);
        Assert.Equal(updated.Name, meal.Name);
        Assert.Equal(updated.User_id, meal.User_id);
        Assert.Equal(updated.Date, meal.Date);
        Assert.Equal(updated.Template, meal.Template);
        Assert.Equal(updated.Image_id, meal.Image_id);
        Assert.Empty(meal.Ingredients);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ShouldDeleteMeal()
    {
        //Arrange
        int userId = 12345;
        int mealId = 42;
        var deleted = CreateMeal(mealId, "Deleted Meal", userId);

        mockRepository.Setup(r => r.DeleteAsync(mealId))
            .ReturnsAsync(1);
        mockRepository.Setup(r => r.GetByIdAsync(mealId))
            .ReturnsAsync(deleted);
        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId))
            .ReturnsAsync([]);

        //Act
        var result = await controller.Delete(mealId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meal = Assert.IsType<MealWithIngredientsDTO>(okResult.Value);

        Assert.Equal(deleted.Id, meal.Id);
        Assert.Equal(deleted.Name, meal.Name);
        Assert.Equal(deleted.User_id, meal.User_id);
        Assert.Equal(deleted.Date, meal.Date);
        Assert.Equal(deleted.Template, meal.Template);
        Assert.Equal(deleted.Image_id, meal.Image_id);
        Assert.Empty(meal.Ingredients);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllTemp_ShouldGetAllTemplates()
    {
        //Arrange
        int userId = 12345;
        string token = "Bearer test-token";
        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());
        int mealId1 = 42;
        int mealId2 = 32;
        var expected1 = CreateMeal(mealId1, "Test Meal1", userId, true);
        var expected2 = CreateMeal(mealId2, "Test Meal2", userId, true);

        var expectedMeals = new[] { expected1, expected2 };

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId1))
            .ReturnsAsync([]);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId2))
            .ReturnsAsync([]);

        mockRepository.Setup(r => r.GetAllTemplatesByUserAsync(userId))
            .ReturnsAsync(expectedMeals);

        //Act
        var result = await controller.GetAllTemplates(token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();

        Assert.Equal(2, meals.Count);
        Assert.Equal(expected1.Id, meals[0].Id);
        Assert.Equal(expected1.Name, meals[0].Name);
        Assert.Equal(expected2.Id, meals[1].Id);
        Assert.Equal(expected2.Name, meals[1].Name);

        Assert.All(meals, meal =>
        {
            Assert.Equal(userId, meal.User_id);
            Assert.True(meal.Template);
            Assert.Empty(meal.Ingredients);
        });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsyncTemp_ShouldUpdateTemplate()
    {
        // Arrange
        int userId = 12345;
        int mealId = 42;
        string token = "Bearer test-token";
        bool template = true;

        var original = CreateMeal(mealId, "Test Meal", userId, false);

        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        mockRepository.Setup(r => r.GetByIdAsync(mealId))
            .ReturnsAsync(original);

        mockRepository.Setup(r => r.UpdateTemplateStatusAsync(mealId, template))
            .ReturnsAsync(1);

        // Act
        var result = await controller.UpdateTemplateStatus(token, mealId, template);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var message = okResult.Value.GetType()
            .GetProperty("Message")?.GetValue(okResult.Value);

        Assert.Equal("Template status updated successfully", message);

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(token), Times.Once);
        mockRepository.Verify(r => r.GetByIdAsync(mealId), Times.Once);
        mockRepository.Verify(r => r.UpdateTemplateStatusAsync(mealId, template), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Get_ShouldGetUniqueIngredientFromMeal()
    {
        // Arrange
        int userId = 12345;
        string token = "Bearer test-token";

        int mealId1 = 42;
        int mealId2 = 32;

        var meal1 = CreateMeal(mealId1, "Test Meal1", userId);
        var meal2 = CreateMeal(mealId2, "Test Meal2", userId);

        var mealIds = new List<int> { mealId1, mealId2 };

        var ingredient1 = new IngredientDTO
        {
            Id = 1,
            Name = "Bread",
            User_id = userId,
            Image_id = null
        };

        var ingredient2 = new IngredientDTO
        {
            Id = 2,
            Name = "Cheese",
            User_id = userId,
            Image_id = null
        };

        var ingredient3 = new IngredientDTO
        {
            Id = 3,
            Name = "Ham",
            User_id = userId,
            Image_id = null
        };

        var packedMeal1 = new[]
        {
            new PackedDTO { Meal_id = mealId1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO { Meal_id = mealId1, Ingredient_id = 2, order_no = 2 }
        };

        var packedMeal2 = new[]
        {
            new PackedDTO { Meal_id = mealId2, Ingredient_id = 2, order_no = 1 },
            new PackedDTO { Meal_id = mealId2, Ingredient_id = 3, order_no = 2 }
        };

        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        mockRepository.Setup(r => r.GetByIdAsync(mealId1))
            .ReturnsAsync(meal1);

        mockRepository.Setup(r => r.GetByIdAsync(mealId2))
            .ReturnsAsync(meal2);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId1))
            .ReturnsAsync(packedMeal1);

        mockPackedRepository.Setup(r => r.GetAllByMealIdAsync(mealId2))
            .ReturnsAsync(packedMeal2);

        mockIngredientsRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(ingredient1);

        mockIngredientsRepository.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(ingredient2);

        mockIngredientsRepository.Setup(r => r.GetByIdAsync(3))
            .ReturnsAsync(ingredient3);

        // Act
        var result = await controller.GetUniqueIngredientsFromMeals(token, mealIds);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ingredients = Assert.IsAssignableFrom<IEnumerable<IngredientDTO>>(okResult.Value).ToList();

        Assert.Equal(3, ingredients.Count);

        Assert.Contains(ingredients, i => i.Id == ingredient1.Id && i.Name == ingredient1.Name);
        Assert.Contains(ingredients, i => i.Id == ingredient2.Id && i.Name == ingredient2.Name);
        Assert.Contains(ingredients, i => i.Id == ingredient3.Id && i.Name == ingredient3.Name);

        Assert.All(ingredients, ingredient =>
            Assert.Equal(userId, ingredient.User_id));

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(token), Times.Once);

        mockRepository.Verify(r => r.GetByIdAsync(mealId1), Times.Once);
        mockRepository.Verify(r => r.GetByIdAsync(mealId2), Times.Once);

        mockPackedRepository.Verify(r => r.GetAllByMealIdAsync(mealId1), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllByMealIdAsync(mealId2), Times.Once);

        mockIngredientsRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        mockIngredientsRepository.Verify(r => r.GetByIdAsync(2), Times.Exactly(2));
        mockIngredientsRepository.Verify(r => r.GetByIdAsync(3), Times.Once);
    }

    private static MealDTO CreateMeal(int id, string name, int userId, bool template = false)
    {
        return new MealDTO
        {
            Id = id,
            Name = name,
            User_id = userId,
            Date = "2026-11-16",
            Template = template,
            Image_id = null
        };
    }
}
