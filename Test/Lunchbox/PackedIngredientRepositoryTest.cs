using Moq;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using Xunit;

public class PackedIngredientRepositoryTests
{
    // private readonly Mock<PostgreSQLConnectionFactory> _connectionFactoryMock;
    // private readonly Mock<NpgsqlConnection> _dbConnectionMock;
    // private readonly PackedIngredientRepository _repository;

    // public PackedIngredientRepositoryTests()
    // {
    //     _connectionFactoryMock = new Mock<PostgreSQLConnectionFactory>();
    //     _dbConnectionMock = new Mock<NpgsqlConnection>();
    //     _repository = new PackedIngredientRepository(_connectionFactoryMock.Object);
    // }

    // [Fact]
    // public async Task GetAllAsync_ReturnAllPackedIngredients()
    // {
    //     // Arrange
    //     var packedIngredients = new List<PackedIngredient>
    //     {
    //         new PackedIngredient { Id = 1, Meal_ref = "Meal1", Ingredient_ref = "Ingredient1" },
    //         new PackedIngredient { Id = 2, Meal_ref = "Meal2", Ingredient_ref = "Ingredient2" }
    //     };

    //     _connectionFactoryMock.Setup(cf => cf.Create()).Returns(_dbConnectionMock.Object);
    //     _dbConnectionMock.Setup(db => db.QueryAsync<PackedIngredient>(
    //         It.IsAny<string>(), 
    //         null, 
    //         null, 
    //         null, 
    //         null, 
    //         null))
    //         .ReturnsAsync(packedIngredients);

    //     // Act
    //     var result = await _repository.GetAllAsync();

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(2, result.Count());
    // }

    // [Fact]
    // public async Task GetByIdAsync_ReturnPackedIngredient_WhenExists()
    // {
    //     // Arrange
    //     var packedIngredient = new PackedIngredient { Id = 1, Meal_ref = "Meal1", Ingredient_ref = "Ingredient1" };

    //     _connectionFactoryMock.Setup(cf => cf.Create()).Returns(_dbConnectionMock.Object);
    //     _dbConnectionMock.Setup(db => db.QuerySingleOrDefaultAsync<PackedIngredient>(
    //         It.IsAny<string>(), 
    //         null, 
    //         null, 
    //         null, 
    //         null, 
    //         null))
    //         .ReturnsAsync(packedIngredient);

    //     // Act
    //     var result = await _repository.GetByIdAsync(1);

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(1, result.Id);
    // }

    // [Fact]
    // public async Task InsertAsync_InsertPackedIngredient()
    // {
    //     // Arrange
    //     var packedIngredient = new PackedIngredient { Meal_ref = "Meal1", Ingredient_ref = "Ingredient1" };

    //     _connectionFactoryMock.Setup(cf => cf.Create()).Returns(_dbConnectionMock.Object);
    //     _dbConnectionMock.Setup(db => db.ExecuteAsync(
    //         It.IsAny<string>(), 
    //         null, 
    //         null, 
    //         null, 
    //         null))
    //         .ReturnsAsync(1);
        
    //     // Act
    //     var result = await _repository.InsertAsync(packedIngredient);
        
    //     // Assert
    //     Assert.Equal(1, result);
    // }

    // [Fact]
    // public async Task UpdateAsync_UpdatePackedIngredient()
    // {
    //     // Arrange
    //     var packedIngredient = new PackedIngredient { Meal_ref = "UpdatedMeal", Ingredient_ref = "UpdatedIngredient" };

    //     _connectionFactoryMock.Setup(cf => cf.Create()).Returns(_dbConnectionMock.Object);
    //     _dbConnectionMock.Setup(db => db.ExecuteAsync(
    //         It.IsAny<string>(), 
    //         null, 
    //         null, 
    //         null, 
    //         null))
    //         .ReturnsAsync(1);
        
    //     // Act
    //     var result = await _repository.UpdateAsync(packedIngredient, 1);
        
    //     // Assert
    //     Assert.Equal(1, result);
    // }

    // [Fact]
    // public async Task DeleteAsync_DeletePackedIngredient()
    // {
    //     // Arrange
    //     _connectionFactoryMock.Setup(cf => cf.Create()).Returns(_dbConnectionMock.Object);
    //     _dbConnectionMock.Setup(db => db.ExecuteAsync(
    //         It.IsAny<string>(), 
    //         null, 
    //         null, 
    //         null, 
    //         null))
    //         .ReturnsAsync(1);
        
    //     // Act
    //     var result = await _repository.DeleteAsync(1);
        
    //     // Assert
    //     Assert.Equal(1, result);
    // }
}