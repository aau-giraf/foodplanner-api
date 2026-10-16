using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;

namespace Test.Service;

public class SubIngredientServiceTests
{
    // mock repository and service
    private readonly Mock<SubIngredientRepository> mockSubIngredientRepository = new();
    private readonly SubIngredientService subIngredientService;
    public SubIngredientServiceTests()
    {
        subIngredientService = new SubIngredientService(
            mockSubIngredientRepository.Object
        );
    }

    [Fact]
    public async Task GetAllSubIngredientsAsync_Success()
    {
        // Arrange
        var subIngredients = new List<SubIngredientDTO>
        {
            new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 },
            new SubIngredientDTO { Parent_id = 2, Child_id = 2, Order_number = 2 }
        };
        mockSubIngredientRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(subIngredients);

        // Act
        var result = await subIngredientService.GetAllSubIngredientsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count());
        Assert.Equal(1, resultList[0].Parent_id);
        Assert.Equal(2, resultList[1].Parent_id);
    }

    [Fact]
    public async Task GetAllSubIngredientsAsync_RepositoryError()
    {
        // Arrange
        mockSubIngredientRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => subIngredientService.GetAllSubIngredientsAsync());
    }

    [Fact]
    public async Task GetAllSubIngredientsByParentIdAsync_Success()
    {
        // Arrange
        int parentId = 1;
        var subIngredients = new List<SubIngredientDTO>
        {
            new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 },
            new SubIngredientDTO { Parent_id = 1, Child_id = 2, Order_number = 2 }
        };
        mockSubIngredientRepository.Setup(repo => repo.GetAllByParentIdAsync(parentId)).ReturnsAsync(subIngredients);

        // Act
        var result = await subIngredientService.GetAllSubIngredientsByParentIdAsync(parentId);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count());
        Assert.Equal(1, resultList[0].Child_id);
        Assert.Equal(2, resultList[1].Child_id);
    }

    [Fact]
    public async Task GetAllSubIngredientsByParentIdAsync_RepositoryError()
    {
        // Arrange
        int parentId = 1;
        mockSubIngredientRepository.Setup(repo => repo.GetAllByParentIdAsync(parentId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => subIngredientService.GetAllSubIngredientsByParentIdAsync(parentId));
    }

    [Fact]
    public async Task CreateSubIngredientAsync_Success()
    {
        // Arrange
        var subIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 };
        mockSubIngredientRepository.Setup(repo => repo.InsertAsync(subIngredient)).ReturnsAsync(1);

        // Act
        var result = await subIngredientService.CreateSubIngredientAsync(subIngredient);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreateSubIngredientAsync_RepositoryError()
    {
        // Arrange
        var subIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 };
        mockSubIngredientRepository.Setup(repo => repo.InsertAsync(subIngredient)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => subIngredientService.CreateSubIngredientAsync(subIngredient));
    }

    [Fact]
    public async Task UpdateSubIngredientAsync_Success()
    {
        // Arrange
        var oldSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 };
        var newSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 2 };
        mockSubIngredientRepository.Setup(repo => repo.UpdateAsync(oldSubIngredient, newSubIngredient)).ReturnsAsync(1);

        // Act
        var result = await subIngredientService.UpdateSubIngredientAsync(oldSubIngredient, newSubIngredient);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdateIngredientAsync_RepositoryError()
    {
        // Arrange
        var oldSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 };
        var newSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 2 };
        mockSubIngredientRepository.Setup(repo => repo.UpdateAsync(oldSubIngredient, newSubIngredient)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => subIngredientService.UpdateSubIngredientAsync(oldSubIngredient, newSubIngredient));
    }

    [Fact]
    public async Task DeleteSubIngredientAsync_Success()
    {
        // Arrange
        var subIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 };
        mockSubIngredientRepository.Setup(repo => repo.DeleteAsync(subIngredient)).ReturnsAsync(1);

        // Act
        var result = await subIngredientService.DeleteSubIngredientAsync(subIngredient);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task DeleteSubIngredientAsync_RepositoryError()
    {
        // Arrange
        var subIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 1, Order_number = 1 };
        mockSubIngredientRepository.Setup(repo => repo.DeleteAsync(subIngredient)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => subIngredientService.DeleteSubIngredientAsync(subIngredient));
    }
}