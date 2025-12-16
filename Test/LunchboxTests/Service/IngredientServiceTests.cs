using AutoMapper;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;
using Test.Builder;

namespace Test.LunchboxTests.Service;

public class IngredientServiceTests
{
    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsAllIngredients()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();
        var ingredient1 = new IngredientBuilder().WithId(1).WithName("Tomato").WithUserId(2).Build();
        var ingredient2 = new IngredientBuilder().WithId(2).WithName("Cheese").WithUserId(1).Build();

        var ingredients = new List<Ingredient>
        { ingredient1, ingredient2 };

        mockIngredientRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(ingredients);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<IngredientDTO>(It.IsAny<Ingredient>()))
            .Returns((Ingredient src) => new IngredientDTO { Id = src.Id, Name = src.Name, User_id = src.User_id });

        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await ingredientService.GetAllIngredientsAsync();

        // Assert
        Assert.NotNull(result);

        var expectedIngredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Tomato", User_id = 2 },
            new IngredientDTO { Id = 2, Name = "Cheese", User_id = 1 }
        };
        var expectedCount = 2;
        Assert.Equal(expectedCount, result.Count());

        var expectedIngredientDto1 = new IngredientDTO { Id = 1, Name = "Tomato", User_id = 2 };
        Assert.Equivalent(expectedIngredientDto1, result.ElementAt(0));

        var expectedIngredientDto2 =new IngredientDTO { Id = 2, Name = "Cheese", User_id = 1 };
        Assert.Equivalent(expectedIngredientDto2, result.ElementAt(1));
    }

    [Fact]
    public async Task GetAllIngredientsByUserAsync_ReturnsIngredientsForUser()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        int userId = 123;
        var ingredient1 = new Ingredient { Id = 1, Name = "Lettuce", User_id = userId };
        var ingredient2 = new Ingredient { Id = 2, Name = "Bacon", User_id = userId };

        var expectedIngredients = new List<Ingredient>
        { ingredient1, ingredient2 };

        mockIngredientRepository
            .Setup(repo => repo.GetAllByUserAsync(userId))
            .ReturnsAsync(expectedIngredients);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<IngredientDTO>(It.IsAny<Ingredient>()))
            .Returns((Ingredient src) => new IngredientDTO { Id = src.Id, Name = src.Name, User_id = src.User_id });

        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object );

        // Act
        var result = await ingredientService.GetAllIngredientsByUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedIngredients.Count, result.Count());
        Assert.All(result, ingredient => Assert.Equal(userId, ingredient.User_id));
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredientById()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var ingredient = new Ingredient { Id = 1, Name = "Tomato", User_id = 2 };

        mockIngredientRepository
            .Setup(repo => repo.GetByIdAsync(ingredient.Id))
            .ReturnsAsync(ingredient);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<IngredientDTO>(It.IsAny<Ingredient>()))
            .Returns((Ingredient src) => new IngredientDTO { Id = src.Id, Name = src.Name, User_id = src.User_id });

        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await ingredientService.GetIngredientByIdAsync(ingredient.Id);

        // Assert
        Assert.NotNull(result);

        var expectedIngredientId = 1;
        Assert.Equal(expectedIngredientId, result.Id);

        var expectedIngredientName = "Tomato";
        Assert.Equal(expectedIngredientName, result.Name);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull_WhenIngredientDoesNotExist()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        int nonExistingId = 999;
        _ = mockIngredientRepository
            .Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Ingredient?)null); // Simulate non-existent ingredient
        var mockMapper = new Mock<IMapper>();

        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await ingredientService.GetIngredientByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);  // Should return null as ingredient doesn't exist
    }

    [Fact]
    public async Task CreateIngredientAsync_ReturnsNewIngredientId()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var newIngredient = new Ingredient { Id = 3, Name = "Onion", User_id = 2 };
        int newIngredientId = 42;

        mockIngredientRepository
            .Setup(repo => repo.InsertAsync(It.IsAny<Ingredient>()))
            .ReturnsAsync(newIngredientId);

        var mockMapper = new Mock<IMapper>();
        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await ingredientService.CreateIngredientAsync(new IngredientDTO
        {
            Name = "Onion",
            Food_image_id = 2,
            Id = 0,
            User_id = 0
        }, 2);

        // Assert
        Assert.Equal(newIngredientId, result);
    }

    [Fact]
    public async Task UpdateIngredientAsync_UpdatesIngredient_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var ingredient = new Ingredient { Id = 1, Name = "Updated Tomato", User_id = 2 };
        int rowsAffected = 1;

        mockIngredientRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Ingredient>()))
            .ReturnsAsync(rowsAffected);

        var mockMapper = new Mock<IMapper>();
        var ingredientDto = new IngredientDTO { Id = 1, Name = "Updated Tomato", User_id = 2 };

        // Service will map from DTO to entity for update; set up mapper accordingly.
        mockMapper
            .Setup(m => m.Map<Ingredient>(It.IsAny<IngredientDTO>()))
            .Returns(ingredient);

        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await ingredientService.UpdateIngredientAsync(ingredientDto, ingredient.Id);

        // Assert
        Assert.Equal(rowsAffected, result);
    }

    [Fact]
    public async Task DeleteIngredientAsync_DeletesIngredient_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        int ingredientIdToDelete = 1;
        int rowsAffected = 1;

        mockIngredientRepository
            .Setup(repo => repo.DeleteAsync(ingredientIdToDelete))
            .ReturnsAsync(rowsAffected);

        var mockMapper = new Mock<IMapper>();
        var ingredientService = new IngredientService(mockIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await ingredientService.DeleteIngredientAsync(ingredientIdToDelete);

        // Assert
        Assert.Equal(rowsAffected, result);
    }
}