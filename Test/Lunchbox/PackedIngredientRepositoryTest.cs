using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using Xunit;

namespace testing;
public class PackedIngredientRepositoryTests
{
    // private readonly PackedIngredientRepository _repository;

    // public PackedIngredientRepositoryTests()
    // {
    //     // Initialize the connection factory using DatabaseConnection
    //     var connectionFactory = DatabaseConnection.GetConnection();
    //     _repository = new PackedIngredientRepository(connectionFactory);
    // }

    // [Fact]
    // public async Task GetAllAsync_ReturnsAllPackedIngredients()
    // {
    //     // Arrange
    //     var sql = "SELECT * FROM packed_ingredients";

    //     // Act
    //     using var connection = DatabaseConnection.GetConnection().Create();
    //     connection.Open();
    //     IEnumerable<PackedIngredient> result = await connection.QueryAsync<PackedIngredient>(sql);

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.True(result.Any());
    // }

    // [Fact]
    // public async Task GetByIdAsync_ReturnsPackedIngredient_WhenExists()
    // {
    //     // Arrange
    //     var id = 5;
    //     var sql = "SELECT * FROM packed_ingredients WHERE id = @Id";

    //     // Act
    //     using var connection = DatabaseConnection.GetConnection().Create();
    //     connection.Open();
    //     var result = await connection.QuerySingleOrDefaultAsync<PackedIngredient>(sql, new { Id = id });

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(id, result.Id);
    // }

    // [Fact]
    // public async Task InsertAsync_InsertsPackedIngredient()
    // {
    //     // Arrange
    //     var packedIngredient = new PackedIngredient { Id = 5, Meal_ref = 19, Ingredient_ref = 3 };
    //     var sql = "INSERT INTO packed_ingredients (meal_ref, ingredient_ref) VALUES (@Meal_ref, @Ingredient_ref)";

    //     // Act
    //     using var connection = DatabaseConnection.GetConnection().Create();
    //     connection.Open();
    //     var result = await connection.ExecuteAsync(sql, packedIngredient);

    //     // Assert
    //     Assert.Equal(1, result);
    // }


    // [Fact]
    // public async Task UpdateAsync_UpdatesPackedIngredient()
    // {
    //     // Arrange
    //     var packedIngredient = new PackedIngredient { Id = 5, Meal_ref = 19, Ingredient_ref = 2 };
    //     var id = 6; // Use a valid existing ID
    //     var sql = "UPDATE packed_ingredients SET meal_ref = @Meal_ref, ingredient_ref = @Ingredient_ref WHERE id = @Id";

    //     // Act
    //     using var connection = DatabaseConnection.GetConnection().Create();
    //     connection.Open();
    //     var result = await connection.ExecuteAsync(sql, new { packedIngredient.Meal_ref, packedIngredient.Ingredient_ref, Id = id });

    //     // Assert
    //     Assert.Equal(1, result);
    // }

    // [Fact]
    // public async Task DeleteAsync_DeletesPackedIngredient()
    // {
    //     // Arrange
    //     var id = 6; // Use a valid existing ID
    //     var sql = "DELETE FROM packed_ingredients WHERE id = @Id";

    //     // Act
    //     using var connection = DatabaseConnection.GetConnection().Create();
    //     connection.Open();
    //     var result = await connection.ExecuteAsync(sql, new { Id = id });

    //     // Assert
    //     Assert.Equal(1, result);  // Should return 1 affected row
    // }
}
