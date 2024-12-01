using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;
using Xunit;

public class PackedIngredientServiceTests
{
    [Fact]
    public async Task GetAllPackedIngredientsAsync_ReturnsAllPackedIngredients()
    {
        // Arrange
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockMapper = new Mock<IMapper>();

        // Data returned by repository mock
        var packedIngredients = new List<PackedIngredient>
        {
            new PackedIngredient { Id = 1, Meal_id = 101, Ingredient_id = 201, order_number = 2 },
            new PackedIngredient { Id = 2, Meal_id = 102, Ingredient_id = 202, order_number = 1 }
        };

        // Expected data mapped by AutoMapper
        var PackedIngredientProperDTOs = new List<PackedIngredientProperDTO>
        {
            new PackedIngredientProperDTO { Meal_id = 101, Ingredient_id =  2 },
            new PackedIngredientProperDTO { Meal_id = 102, Ingredient_id = 1 }
        };

        // Mock the repository behavior
        mockPackedIngredientRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(packedIngredients);

        // Mock the mapping behavior
        mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<PackedIngredientProperDTO>>(packedIngredients))
            .Returns(PackedIngredientProperDTOs);

        var packedIngredientService = new PackedIngredientService(mockPackedIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await packedIngredientService.GetAllPackedIngredientsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PackedIngredientProperDTOs.Count, result.Count()); // Checks that the count matches
    }

    [Fact]
    public async Task GetAllPackedIngredientsByMealIdAsync_ReturnsPackedIngredientsForMeal()
    {
        // Arrange
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockMapper = new Mock<IMapper>();

        int mealId = 101;
        var packedIngredients = new List<PackedIngredient>
        {
            new PackedIngredient { Id = 1, Meal_id = mealId, Ingredient_id = 201, order_number = 1 },
            new PackedIngredient { Id = 2, Meal_id = mealId, Ingredient_id = 202, order_number = 2 }
        };

        var packedIngredientDTOs = new List<PackedIngredientDTO>
        {
            new PackedIngredientDTO { Id = 1, Meal_id = mealId, Ingredient_id = new Ingredient{ Id = 1, Name = "jbsfljbatest", User_id = 2 }, order_number = 1 },
            new PackedIngredientDTO { Id = 2, Meal_id = mealId, Ingredient_id = new Ingredient{ Id = 1, Name = "rottests", User_id = 2 }, order_number = 2 }
        };

        mockPackedIngredientRepository
            .Setup(repo => repo.GetAllByMealIdAsync(mealId))
            .ReturnsAsync(packedIngredients);

        mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<PackedIngredientDTO>>(packedIngredients))
            .Returns(packedIngredientDTOs);

        var packedIngredientService = new PackedIngredientService(mockPackedIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await packedIngredientService.GetAllPackedIngredientsByMealIdAsync(mealId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(packedIngredientDTOs.Count, result.Count());
        Assert.All(result, dto => Assert.Equal(mealId, dto.Meal_id));
    }

    [Fact]
    public async Task GetPackedIngredientByIdAsync_ReturnsPackedIngredient()
    {
        // Arrange
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockMapper = new Mock<IMapper>();

        var packedIngredient = new PackedIngredient { Id = 1, Meal_id = 101, Ingredient_id = 201, order_number = 5 };

        mockPackedIngredientRepository
            .Setup(repo => repo.GetByIdAsync(packedIngredient.Id))
            .ReturnsAsync(packedIngredient);

        var packedIngredientService = new PackedIngredientService(mockPackedIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await packedIngredientService.GetPackedIngredientByIdAsync(packedIngredient.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(packedIngredient.Id, result.Id);
        Assert.Equal(packedIngredient.Meal_id, result.Meal_id);
        Assert.Equal(packedIngredient.Ingredient_id, result.Ingredient_id);
    }

    [Fact]
    public async Task CreatePackedIngredientAsync_ReturnsNewPackedIngredientId()
    {
        // Arrange
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockMapper = new Mock<IMapper>();

        int mealRef = 101, ingredientRef = 201;
        int newPackedIngredientId = 42;

        mockPackedIngredientRepository
            .Setup(repo => repo.InsertAsync(mealRef, ingredientRef))
            .ReturnsAsync(newPackedIngredientId);

        var packedIngredientService = new PackedIngredientService(mockPackedIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await packedIngredientService.CreatePackedIngredientAsync(mealRef, ingredientRef);

        // Assert
        Assert.Equal(newPackedIngredientId, result);
    }

    [Fact]
    public async Task UpdatePackedIngredientAsync_UpdatesPackedIngredient_ReturnsAffectedRows()
    {
        // Arrange
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockMapper = new Mock<IMapper>();

        var packedIngredient = new PackedIngredient { Id = 1, Meal_id = 101, Ingredient_id = 201, order_number = 1 };
        int rowsAffected = 1;

        mockPackedIngredientRepository
            .Setup(repo => repo.UpdateAsync(packedIngredient, packedIngredient.Id))
            .ReturnsAsync(rowsAffected);

        var packedIngredientService = new PackedIngredientService(mockPackedIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await packedIngredientService.UpdatePackedIngredientAsync(packedIngredient, packedIngredient.Id);

        // Assert
        Assert.Equal(rowsAffected, result);
    }

    [Fact]
    public async Task DeletePackedIngredientAsync_DeletesPackedIngredient_ReturnsAffectedRows()
    {
        // Arrange
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockMapper = new Mock<IMapper>();

        int packedIngredientId = 1;
        int rowsAffected = 1;

        mockPackedIngredientRepository
            .Setup(repo => repo.DeleteAsync(packedIngredientId))
            .ReturnsAsync(rowsAffected);

        var packedIngredientService = new PackedIngredientService(mockPackedIngredientRepository.Object, mockMapper.Object);

        // Act
        var result = await packedIngredientService.DeletePackedIngredientAsync(packedIngredientId);

        // Assert
        Assert.Equal(rowsAffected, result);
    }
}
