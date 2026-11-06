using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerDataAccessSql;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;

namespace Test.Integration.Services;

[Collection("IntegrationDatabase")]
public class PackedTests
{
    private readonly Mock<IDapperExecutor> mockDapperExecutor = new();
    private readonly PackedRepository packedRepository;
    private readonly PackedService service;
    public PackedTests()
    {
        packedRepository = new PackedRepository(mockDapperExecutor.Object);
        service = new PackedService(packedRepository);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllPackedAsync()
    {
        // Arrange
        var expected = new List<PackedDTO>
        {
            new() { Meal_id = 1, Ingredient_id = 10, order_no = 1 },
            new() { Meal_id = 1, Ingredient_id = 11, order_no = 2 },
            new() { Meal_id = 2, Ingredient_id = 12, order_no = 1 }
        };
        mockDapperExecutor.Setup(x => x.QueryAsync<PackedDTO>("SELECT * FROM packed")).ReturnsAsync(expected);

        // Act
        var result = await service.GetAllPackedAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Count, result.Count());

        Assert.Collection(result,
            item =>
            {
                Assert.Equal(1, item.Meal_id);
                Assert.Equal(10, item.Ingredient_id);
                Assert.Equal(1, item.order_no);
            },
            item =>
            {
                Assert.Equal(1, item.Meal_id);
                Assert.Equal(11, item.Ingredient_id);
                Assert.Equal(2, item.order_no);
            },
            item =>
            {
                Assert.Equal(2, item.Meal_id);
                Assert.Equal(12, item.Ingredient_id);
                Assert.Equal(1, item.order_no);
            });
    }
 
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllPackedByMealIdAsync()
    {
        // Arrange
        int id = 1;
        var expected = new List<PackedDTO>
        {
            new() { Meal_id = id, Ingredient_id = 10, order_no = 1 },
            new() { Meal_id = id, Ingredient_id = 11, order_no = 2 }
        };
        mockDapperExecutor.Setup(x => x.QueryAsync<PackedDTO>(
            "SELECT * FROM packed WHERE meal_id = @Id ORDER BY order_no",
            It.Is<object>(param => (int)param.GetType().GetProperty("Id")!.GetValue(param)! == id)))
            .ReturnsAsync(expected);

        // Act
        var result = await service.GetAllPackedByMealIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Count, result.Count());

        Assert.Collection(result,
            item =>
            {
                Assert.Equal(id, item.Meal_id);
                Assert.Equal(10, item.Ingredient_id);
                Assert.Equal(1, item.order_no);
            },
            item =>
            {
                Assert.Equal(id, item.Meal_id);
                Assert.Equal(11, item.Ingredient_id);
                Assert.Equal(2, item.order_no);
            });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreatePackedAsync()
    {
        // Arrange
        var packed = new PackedDTO { Meal_id = 1, Ingredient_id = 10, order_no = 1 };
        mockDapperExecutor.Setup(x => x.ExecuteAsync(
            "INSERT INTO packed (meal_id, ingredient_id, order_no) VALUES (@MealId, @IngredientId, @OrderNumber)",
            It.Is<object>(param =>
                (int)param.GetType().GetProperty("MealId")!.GetValue(param)! == packed.Meal_id &&
                (int)param.GetType().GetProperty("IngredientId")!.GetValue(param)! == packed.Ingredient_id &&
                (int)param.GetType().GetProperty("OrderNumber")!.GetValue(param)! == packed.order_no)))
            .ReturnsAsync(1);

        // Act
        var result = await service.CreatePackedAsync(packed);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdatePackedAsync()
    {
        // Arrange
        var oldPacked = new PackedDTO { Meal_id = 1, Ingredient_id = 10, order_no = 1 };
        var newPacked = new PackedDTO { Meal_id = 1, Ingredient_id = 10, order_no = 2 };
        mockDapperExecutor.Setup(x => x.ExecuteAsync(
            "UPDATE packed SET meal_id = @NewMealId, ingredient_id = @NewIngredientId, order_no = @NewOrderNumber WHERE meal_id = @OldMealId AND ingredient_id = @OldIngredientId",
            It.Is<object>(param =>
                (int)param.GetType().GetProperty("NewMealId")!.GetValue(param)! == newPacked.Meal_id &&
                (int)param.GetType().GetProperty("NewIngredientId")!.GetValue(param)! == newPacked.Ingredient_id &&
                (int)param.GetType().GetProperty("NewOrderNumber")!.GetValue(param)! == newPacked.order_no &&
                (int)param.GetType().GetProperty("OldMealId")!.GetValue(param)! == oldPacked.Meal_id &&
                (int)param.GetType().GetProperty("OldIngredientId")!.GetValue(param)! == oldPacked.Ingredient_id)))
            .ReturnsAsync(1);

        // Act
        var result = await service.UpdatePackedAsync(oldPacked, newPacked);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeletePackedAsync()
    {
        // Arrange
        var packed = new PackedDTO { Meal_id = 1, Ingredient_id = 10, order_no = 1 };
        mockDapperExecutor.Setup(x => x.ExecuteAsync(
            "DELETE FROM packed WHERE meal_id = @MealId AND ingredient_id = @IngredientId",
            It.Is<object>(param =>
                (int)param.GetType().GetProperty("MealId")!.GetValue(param)! == packed.Meal_id &&
                (int)param.GetType().GetProperty("IngredientId")!.GetValue(param)! == packed.Ingredient_id)))
            .ReturnsAsync(1);

        // Act
        var result = await service.DeletePackedAsync(packed);

        // Assert
        Assert.Equal(1, result);
    }
}