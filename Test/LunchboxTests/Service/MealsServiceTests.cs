using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;

namespace Test.Service;

public class MealsServiceTests
{
    // Mock repositories and service
    private readonly Mock<MealsRepository> mockMealsRepository = new();
    private readonly Mock<PackedRepository> mockPackedRepository = new();
    private readonly Mock<IngredientsRepository> mockIngredientsRepository = new();
    private readonly MealsService mealsService;
    public MealsServiceTests()
    {
        mealsService = new MealsService(
            mockMealsRepository.Object,
            mockPackedRepository.Object,
            mockIngredientsRepository.Object
        );
    }

    [Fact]
    public async Task GetAllMealsAsync_Success()
    {
    // Arrange
        var meals = new List<MealDTO>
        {
            new MealDTO
            {
                Id = 1,
                Name = "Pizza",
                Date = "18/11/2022",
                Template = false,
                Image_id = null,
                User_id = 1
            },
            new MealDTO
            {
                Id = 2,
                Name = "Burger",
                Date = "19/01/2013",
                Template = true,
                Image_id = null,
                User_id = 2
            }
        };
        var packed = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO {Meal_id = 1, Ingredient_id = 2, order_no = 2 },
            new PackedDTO {Meal_id = 2, Ingredient_id = 1, order_no = 1 }
        };
        var ingredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Cheese", User_id = 2 },
            new IngredientDTO { Id = 2, Name = "Tomato", User_id = 1 }
        };
        mockMealsRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(meals);
        mockPackedRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(packed);
        mockIngredientsRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(ingredients);

        // Act
        var result = await mealsService.GetAllMealsAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(meals.Count(), resultList.Count());
        var firstMeal = resultList[0];
        Assert.Equal(1, firstMeal.Id);
        Assert.Equal("Pizza", firstMeal.Name);
        Assert.Equal("18/11/2022", firstMeal.Date);
        Assert.False(firstMeal.Template);
        Assert.Null(firstMeal.Image_id);
        Assert.Equal(1, firstMeal.User_id);
        Assert.Equal(2, firstMeal.Ingredients.Count());
        Assert.Equal("Cheese", firstMeal.Ingredients[0].Name);
        Assert.Equal("Tomato", firstMeal.Ingredients[1].Name);
        var secondMeal = resultList[1];
        Assert.Equal(2, secondMeal.Id);
        Assert.Equal("Burger", secondMeal.Name);
        Assert.Equal("19/01/2013", secondMeal.Date);
        Assert.True(secondMeal.Template);
        Assert.Null(secondMeal.Image_id);
        Assert.Equal(2, secondMeal.User_id);
        Assert.Single(secondMeal.Ingredients);
        Assert.Equal("Cheese", secondMeal.Ingredients[0].Name);
    }

    [Fact]
    public async Task GetAllMealsAsync_RepositoryError()
    {
        // Arrange
        mockMealsRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.GetAllMealsAsync());
    }

    [Fact]
    public async Task GetAllMealsByUserAndDateAsync_Success()
    {
        // Arrange
        int userId = 1;
        string date = "18/11/2022";
        var meals = new List<MealDTO>
        {
            new MealDTO
            {
                Id = 1,
                Name = "Pizza",
                Date = date,
                Template = false,
                Image_id = null,
                User_id = userId
            }
        };
        var packed = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO {Meal_id = 1, Ingredient_id = 2, order_no = 2 }
        };
        var ingredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Cheese", User_id = userId },
            new IngredientDTO { Id = 2, Name = "Tomato", User_id = userId }
        };
        mockMealsRepository.Setup(repo => repo.GetAllByUserAndDateAsync(userId, date)).ReturnsAsync(meals);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(1)).ReturnsAsync(packed);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(ingredients[0]);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(ingredients[1]);

        // Act
        var result = await mealsService.GetAllMealsByUserAndDateAsync(userId, date);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Single(resultList);
        var mealWithIngredients = resultList[0];
        Assert.Equal(1, mealWithIngredients.Id);
        Assert.Equal("Pizza", mealWithIngredients.Name);
        Assert.Equal(date, mealWithIngredients.Date);
        Assert.False(mealWithIngredients.Template);
        Assert.Null(mealWithIngredients.Image_id);
        Assert.Equal(userId, mealWithIngredients.User_id);
        Assert.Equal(2, mealWithIngredients.Ingredients.Count());
    }

    [Fact]
    public async Task GetAllMealsByUserAndDateAsync_RepositoryError()
    {
        // Arrange
        int userId = 1;
        string date = "18/11/2022";
        mockMealsRepository.Setup(repo => repo.GetAllByUserAndDateAsync(userId, date)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.GetAllMealsByUserAndDateAsync(userId, date));
    }

    [Fact]
    public async Task GetMealByIdAsync_Success()
    {
        // Arrange
        var meal = new MealDTO
        {
            Id = 1,
            Name = "Pasta",
            Date = "20/03/2022",
            Template = false,
            Image_id = null,
            User_id = 3
        };
        var packed = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO {Meal_id = 1, Ingredient_id = 2, order_no = 2 }
        };
        var ingredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Noodles", User_id = 3 },
            new IngredientDTO { Id = 2, Name = "Sauce", User_id = 3 }
        };
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(meal.Id)).ReturnsAsync(meal);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(meal.Id)).ReturnsAsync(packed);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(ingredients[0]);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(ingredients[1]);

        // Act
        var result = await mealsService.GetMealByIdAsync(meal.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Pasta", result.Name);
        Assert.Equal("20/03/2022", result.Date);
        Assert.False(result.Template);
        Assert.Null(result.Image_id);
        Assert.Equal(3, result.User_id);
        Assert.Equal(2, result.Ingredients.Count());
        Assert.Equal("Noodles", result.Ingredients[0].Name);
        Assert.Equal("Sauce", result.Ingredients[1].Name);
    }

    [Fact]
    public async Task GetMealByIdAsync_MealDoesNotExist()
    {
        // Arrange
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((MealDTO?)null);

        // Act
        var result = await mealsService.GetMealByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMealByIdAsync_RepositoryError()
    {
        // Arrange
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.GetMealByIdAsync(1));
    }
    
    [Fact]
    public async Task CreateMealAsync_Success()
    {
        // Arrange
        var mealCreateDTO = new MealCreateDTO
        {
            Name = "Salad",
            Date = "21/03/2023",
            Template = false,
            Food_image_id = null
        };
        int newMealId = 1;
        mockMealsRepository.Setup(repo => repo.InsertAsync(It.IsAny<MealCreateDTO>(), It.IsAny<int>())).ReturnsAsync(newMealId);

        // Act
        var result = await mealsService.CreateMealAsync(mealCreateDTO, 1);

        // Assert
        Assert.Equal(newMealId, result);
    }

    [Fact]
    public async Task CreateMealAsync_RepositoryError()
    {
        // Arrange
        var mealCreateDTO = new MealCreateDTO
        {
            Name = "Salad",
            Date = "21/03/2023",
            Template = false,
            Food_image_id = null
        };
        mockMealsRepository.Setup(repo => repo.InsertAsync(It.IsAny<MealCreateDTO>(), It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.CreateMealAsync(mealCreateDTO, 1));
    }

    [Fact]
    public async Task UpdateMealAsync_Success()
    {
        // Arrange
        var meal = new MealDTO
        {
            Id = 1,
            Name = "Updated Meal",
            Date = "22/03/2023",
            Template = true,
            Image_id = null,
            User_id = 1
        };
        mockMealsRepository.Setup(repo => repo.UpdateAsync(meal)).ReturnsAsync(1);

        // Act
        var result = await mealsService.UpdateMealAsync(meal);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdateMealAsync_RepositoryError()
    {
        // Arrange
        var meal = new MealDTO
        {
            Id = 1,
            Name = "Updated Meal",
            Date = "22/03/2023",
            Template = true,
            Image_id = null,
            User_id = 1
        };
        mockMealsRepository.Setup(repo => repo.UpdateAsync(meal)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.UpdateMealAsync(meal));
    }

    [Fact]
    public async Task DeleteMealAsync_Success()
    {
        // Arrange
        int mealId = 1;
        mockMealsRepository.Setup(repo => repo.DeleteAsync(mealId)).ReturnsAsync(1);

        // Act
        var result = await mealsService.DeleteMealAsync(mealId);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task DeleteMealAsync_RepositoryError()
    {
        // Arrange
        int mealId = 1;
        mockMealsRepository.Setup(repo => repo.DeleteAsync(mealId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.DeleteMealAsync(mealId));
    }

    [Fact]
    public async Task GetAllTemplatesByUserAsync_Success()
    {
        // Arrange
        int userId = 1;
        var meals = new List<MealDTO>
        {
            new MealDTO
            {
                Id = 1,
                Name = "Template Meal",
                Date = "23/03/2023",
                Template = true,
                Image_id = null,
                User_id = userId
            }
        };
        mockMealsRepository.Setup(repo => repo.GetAllTemplatesByUserAsync(userId)).ReturnsAsync(meals);

        // Act
        var result = await mealsService.GetAllTemplatesByUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Single(resultList);
        var mealWithIngredients = resultList[0];
        Assert.Equal(1, mealWithIngredients.Id);
        Assert.Equal("Template Meal", mealWithIngredients.Name);
        Assert.Equal("23/03/2023", mealWithIngredients.Date);
        Assert.True(mealWithIngredients.Template);
        Assert.Null(mealWithIngredients.Image_id);
        Assert.Equal(userId, mealWithIngredients.User_id);
    }

    [Fact]
    public async Task GetAllTemplatesByUserAsync_RepositoryError()
    {
        // Arrange
        int userId = 1;
        mockMealsRepository.Setup(repo => repo.GetAllTemplatesByUserAsync(userId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.GetAllTemplatesByUserAsync(userId));
    }

    [Fact]
    public async Task UpdateTemplateStatusAsync_success()
    {
        // Arrange
        int id = 1;
        bool templateStatus = true;
        int userId = 1;
        var meal = new MealDTO
            {
                Id = 1,
                Name = "Template Meal",
                Date = "23/03/2023",
                Template = false,
                Image_id = null,
                User_id = 1
            };
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(meal);
        mockMealsRepository.Setup(repo => repo.UpdateTemplateStatusAsync(userId, templateStatus)).ReturnsAsync(1);

        // Act
        var result = await mealsService.UpdateTemplateStatusAsync(id, templateStatus, userId);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdateTemplateStatusAsync_AuthorizationError()
    {
        // Arrange
        int id = 1;
        bool templateStatus = true;
        int userId = 1;
        var meal = new MealDTO
            {
                Id = 1,
                Name = "Template Meal",
                Date = "23/03/2023",
                Template = false,
                Image_id = null,
                User_id = 2
            };
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(meal);
        mockMealsRepository.Setup(repo => repo.UpdateTemplateStatusAsync(userId, templateStatus)).ReturnsAsync(1);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.UpdateTemplateStatusAsync(id, templateStatus, userId));
    }

    [Fact]
    public async Task UpdateTemplateStatusAsync_MealNotFound()
    {
        // Arrange
        int id = 1;
        bool templateStatus = true;
        int userId = 1;
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((MealDTO?)null);
        mockMealsRepository.Setup(repo => repo.UpdateTemplateStatusAsync(userId, templateStatus)).ReturnsAsync(1);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.UpdateTemplateStatusAsync(id, templateStatus, userId));
    }

    [Fact]
    public async Task GetUniqueIngredientsFromMealsAsync_Success()
    {
        // Arrange
        int userId = 1;
        var mealIds = new List<int> {1,2};
        var meals = new List<MealDTO>
        {
            new MealDTO
            {
                Id = 1,
                Name = "Template Meal 1",
                Date = "23/03/2023",
                Template = false,
                Image_id = null,
                User_id = 1
            },
            new MealDTO
            {
                Id = 2,
                Name = "Template Meal 2",
                Date = "23/03/2023",
                Template = true,
                Image_id = null,
                User_id = 1
            }
        };
        var packed1 = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO {Meal_id = 1, Ingredient_id = 2, order_no = 2 }
        };
        var packed2 = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 2, Ingredient_id = 1, order_no = 1 }
        };
        var Cheese = new IngredientDTO { Id = 1, Name = "Cheese", User_id = 1};
        var Tomato = new IngredientDTO { Id = 2, Name = "Tomato", User_id = 1};
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(meals[0]);
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(meals[1]);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(1)).ReturnsAsync(packed1);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(2)).ReturnsAsync(packed2);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(Cheese);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(Tomato);

        // Act
        var result = await mealsService.GetUniqueIngredientsFromMealsAsync(mealIds, userId);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count());
        Assert.Equal("Cheese", resultList[0].Name);
        Assert.Equal("Tomato", resultList[1].Name);
    }

    [Fact]
    public async Task GetUniqueIngredientsFromMealsAsync_AuthorizationError()
    {
        // Arrange
        int userId = 1;
        var mealIds = new List<int> {1,2};
        var meals = new List<MealDTO>
        {
            new MealDTO
            {
                Id = 1,
                Name = "Template Meal 1",
                Date = "23/03/2023",
                Template = false,
                Image_id = null,
                User_id = 1
            },
            new MealDTO
            {
                Id = 2,
                Name = "Template Meal 2",
                Date = "23/03/2023",
                Template = true,
                Image_id = null,
                User_id = 2
            }
        };
        var packed1 = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO {Meal_id = 1, Ingredient_id = 2, order_no = 2 }
        };
        var packed2 = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 2, Ingredient_id = 1, order_no = 1 }
        };
        var Cheese = new IngredientDTO { Id = 1, Name = "Cheese", User_id = 1};
        var Tomato = new IngredientDTO { Id = 2, Name = "Tomato", User_id = 1};
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(meals[0]);
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(meals[1]);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(1)).ReturnsAsync(packed1);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(2)).ReturnsAsync(packed2);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(Cheese);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(Tomato);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.GetUniqueIngredientsFromMealsAsync(mealIds, userId));
    }

    [Fact]
    public async Task GetUniqueIngredientsFromMealsAsync_RepositoryError()
    {
        // Arrange
        int userId = 1;
        var mealIds = new List<int> {1,2};
        var meals = new List<MealDTO>
        {
            new MealDTO
            {
                Id = 1,
                Name = "Template Meal 1",
                Date = "23/03/2023",
                Template = false,
                Image_id = null,
                User_id = 1
            },
            new MealDTO
            {
                Id = 2,
                Name = "Template Meal 2",
                Date = "23/03/2023",
                Template = true,
                Image_id = null,
                User_id = 1
            }
        };
        var packed1 = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 1, Ingredient_id = 1, order_no = 1 },
            new PackedDTO {Meal_id = 1, Ingredient_id = 2, order_no = 2 }
        };
        var packed2 = new List<PackedDTO>
        {
            new PackedDTO {Meal_id = 2, Ingredient_id = 1, order_no = 1 }
        };
        var Cheese = new IngredientDTO { Id = 1, Name = "Cheese", User_id = 1};
        var Tomato = new IngredientDTO { Id = 2, Name = "Tomato", User_id = 1};
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(meals[0]);
        mockMealsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(meals[1]);
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(1)).ThrowsAsync(new Exception("Database error"));
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(2)).ReturnsAsync(packed2);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(Cheese);
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(Tomato);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mealsService.GetUniqueIngredientsFromMealsAsync(mealIds, userId));
    }
}