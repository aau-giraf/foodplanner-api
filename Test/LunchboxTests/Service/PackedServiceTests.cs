using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;

namespace Test.Service;

public class PackedServiceTests
{
    // mock repository and service
    private readonly Mock<PackedRepository> mockPackedRepository = new();
    private readonly PackedService packedService;
    public PackedServiceTests()
    {
        packedService = new PackedService(
            mockPackedRepository.Object
        );
    }

    [Fact]
    public async Task GetAllPackedAsync_Success()
    {
        // Arrange
        var packed = new List<PackedDTO>
        {
            new PackedDTO { Meal_id = 1, Ingredient_id = 2, Order_number = 1 },
            new PackedDTO { Meal_id = 3, Ingredient_id = 4, Order_number = 2 }
        };
        mockPackedRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(packed);

        // Act
        var result = await packedService.GetAllPackedAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(packed.Count, result.Count());
        Assert.Equal(1, packed[0].Meal_id);
        Assert.Equal(3, packed[1].Meal_id);
    }

    [Fact]
    public async Task GetAllPackedAsync_RepositoryError()
    {
        // Arrange
        mockPackedRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.GetAllPackedAsync());
    }

    [Fact]
    public async Task GetAllPackedByMealIdAsync_Success()
    {
        // Arrange
        int mealId = 1;
        var packed = new List<PackedDTO>
        {
            new PackedDTO { Meal_id = mealId, Ingredient_id = 1, Order_number = 1 },
            new PackedDTO { Meal_id = mealId, Ingredient_id = 2, Order_number = 2 }
        };
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(mealId)).ReturnsAsync(packed);

        // Act
        var result = await packedService.GetAllPackedByMealIdAsync(mealId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(packed.Count, result.Count());
        Assert.Equal(1, packed[0].Ingredient_id);
        Assert.Equal(2, packed[1].Ingredient_id);
    }

    [Fact]
    public async Task GetAllPackedByMealIdAsync_RepositoryError()
    {
        // Arrange
        int mealId = 1;
        mockPackedRepository.Setup(repo => repo.GetAllByMealIdAsync(mealId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.GetAllPackedByMealIdAsync(mealId));
    }

    [Fact]
    public async Task CreatePackedAsync_Success()
    {
        // Arrange
        var packed = new PackedDTO{ Meal_id = 1, Ingredient_id = 2, Order_number= 3 };
        mockPackedRepository.Setup(repo => repo.InsertAsync(packed)).ReturnsAsync(1);

        // Act
        var result = await packedService.CreatePackedAsync(packed);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreatePackedAsync_RepositoryError()
    {
        // Arrange
        var packed = new PackedDTO{ Meal_id = 1, Ingredient_id = 2, Order_number= 3 };
        mockPackedRepository.Setup(repo => repo.InsertAsync(packed)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.CreatePackedAsync(packed));
    }

    [Fact]
    public async Task UpdatePackedAsync_Success()
    {
        // Arrange
        var oldPacked = new PackedDTO { Meal_id = 101, Ingredient_id = 201, Order_number = 1 };
        var newPacked = new PackedDTO { Meal_id = 101, Ingredient_id = 201, Order_number = 2 };
        mockPackedRepository.Setup(repo => repo.UpdateAsync(oldPacked, newPacked)).ReturnsAsync(1);
    
        // Act
        var result = await packedService.UpdatePackedAsync(oldPacked, newPacked);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdatePackedAsync_RepositoryError()
    {
        // Arrange
        var oldPacked = new PackedDTO { Meal_id = 101, Ingredient_id = 201, Order_number = 1 };
        var newPacked = new PackedDTO { Meal_id = 101, Ingredient_id = 201, Order_number = 2 };
        mockPackedRepository.Setup(repo => repo.UpdateAsync(oldPacked, newPacked)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.UpdatePackedAsync(oldPacked, newPacked));
    }

    [Fact]
    public async Task DeletePackedAsync_Success()
    {
        // Arrange
        var toBeDeletedPacked = new PackedDTO { Meal_id = 101, Ingredient_id = 201, Order_number = 1 };
        mockPackedRepository.Setup(repo => repo.DeleteAsync(toBeDeletedPacked)).ReturnsAsync(1);

        // Act
        var result = await packedService.DeletePackedAsync(toBeDeletedPacked);

        // Assert
        Assert.Equal(1, result);
    }
    
    [Fact]
    public async Task DeletePackedAsync_RepositoryError()
    {
        // Arrange
        var toBeDeletedPacked = new PackedDTO { Meal_id = 101, Ingredient_id = 201, Order_number = 1 };
        mockPackedRepository.Setup(repo => repo.DeleteAsync(toBeDeletedPacked)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.DeletePackedAsync(toBeDeletedPacked));
    }
}