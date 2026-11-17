using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;

namespace Test.Unit.Service;

public class PackedServiceTests
{
    // mock repository and service
    private readonly Mock<IPackedRepository> mockPackedRepository = new();
    private readonly PackedService packedService;
    public PackedServiceTests()
    {
        packedService = new PackedService(
            mockPackedRepository.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreatePackedAsync_Success()
    {
        // Arrange
        var packed = new Packed{ Meal_id = 1, Ingredient_id = 2, order_no= 3 };
        mockPackedRepository.Setup(repo => repo.InsertAsync(packed)).ReturnsAsync(1);

        // Act
        var result = await packedService.CreatePackedAsync(packed);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreatePackedAsync_RepositoryError()
    {
        // Arrange
        var packed = new Packed{ Meal_id = 1, Ingredient_id = 2, order_no= 3 };
        mockPackedRepository.Setup(repo => repo.InsertAsync(packed)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.CreatePackedAsync(packed));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdatePackedAsync_Success()
    {
        // Arrange
        var oldPacked = new Packed { Meal_id = 101, Ingredient_id = 201, order_no = 1 };
        var newPacked = new Packed { Meal_id = 101, Ingredient_id = 201, order_no = 2 };
        mockPackedRepository.Setup(repo => repo.UpdateAsync(oldPacked, newPacked)).ReturnsAsync(1);
    
        // Act
        var result = await packedService.UpdatePackedAsync(oldPacked, newPacked);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdatePackedAsync_RepositoryError()
    {
        // Arrange
        var oldPacked = new Packed { Meal_id = 101, Ingredient_id = 201, order_no = 1 };
        var newPacked = new Packed { Meal_id = 101, Ingredient_id = 201, order_no = 2 };
        mockPackedRepository.Setup(repo => repo.UpdateAsync(oldPacked, newPacked)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.UpdatePackedAsync(oldPacked, newPacked));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeletePackedAsync_Success()
    {
        // Arrange
        var toBeDeletedPacked = new Packed { Meal_id = 101, Ingredient_id = 201, order_no = 1 };
        mockPackedRepository.Setup(repo => repo.DeleteAsync(toBeDeletedPacked)).ReturnsAsync(1);

        // Act
        var result = await packedService.DeletePackedAsync(toBeDeletedPacked);

        // Assert
        Assert.Equal(1, result);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeletePackedAsync_RepositoryError()
    {
        // Arrange
        var toBeDeletedPacked = new Packed { Meal_id = 101, Ingredient_id = 201, order_no = 1 };
        mockPackedRepository.Setup(repo => repo.DeleteAsync(toBeDeletedPacked)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => packedService.DeletePackedAsync(toBeDeletedPacked));
    }
}