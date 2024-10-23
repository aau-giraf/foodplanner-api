using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using Xunit;

namespace testing
{
    public class IngredientRepositoryTests
    {
        // private readonly IngredientRepository _repository;

        // public IngredientRepositoryTests()
        // {
        //     // Initialize the connection factory using DatabaseConnection
        //     var connectionFactory = DatabaseConnection.GetConnection();
        //     _repository = new IngredientRepository(connectionFactory);
        // }

        // [Fact]
        // public async Task GetAllAsync_ReturnsAllIngredients()
        // {
        //     // Arrange
        //     var sql = "SELECT * FROM ingredients";

        //     // Act
        //     using var connection = DatabaseConnection.GetConnection().Create();
        //     connection.Open();
        //     IEnumerable<Ingredient> result = await connection.QueryAsync<Ingredient>(sql);

        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.True(result.Any());
        // }

        // [Fact]
        // public async Task GetByIdAsync_ReturnsIngredient_WhenExists()
        // {
        //     // Arrange
        //     var id = 1;  // Assuming there is an ingredient with this ID
        //     var sql = "SELECT * FROM ingredients WHERE id = @Id";

        //     // Act
        //     using var connection = DatabaseConnection.GetConnection().Create();
        //     connection.Open();
        //     var result = await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id });

        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.Equal(id, result.Id);
        // }

        // [Fact]
        // public async Task InsertAsync_InsertsIngredient()
        // {
        //     // Arrange
        //     var ingredient = new Ingredient { Id = 4, Name = "Carrot", User_ref = "user1", Image_ref = "image1" };
        //     var sql = "INSERT INTO ingredients (name, user_ref, image_ref) VALUES (@Name, @User_ref, @Image_ref)";

        //     // Act
        //     using var connection = DatabaseConnection.GetConnection().Create();
        //     connection.Open();
        //     var result = await connection.ExecuteAsync(sql, ingredient);

        //     // Assert
        //     Assert.Equal(1, result);  // Should return 1 affected row
        // }

        // [Fact]
        // public async Task UpdateAsync_UpdatesIngredient()
        // {
        //     // Arrange
        //     var ingredient = new Ingredient { Id = 2, Name = "UpdatedName", User_ref = "user1", Image_ref = "image1" };
        //     var id = 2;  // Use a valid existing ID
        //     var sql = "UPDATE ingredients SET name = @Name, user_ref = @User_ref, image_ref = @Image_ref WHERE id = @Id";

        //     // Act
        //     using var connection = DatabaseConnection.GetConnection().Create();
        //     connection.Open();
        //     var result = await connection.ExecuteAsync(sql, new { ingredient.Name, ingredient.User_ref, ingredient.Image_ref, Id = id });

        //     // Assert
        //     Assert.Equal(1, result);  // Should return 1 affected row
        // }

        // [Fact]
        // public async Task DeleteAsync_DeletesIngredient()
        // {
        //     // Arrange
        //     var id = 2;  // Use a valid existing ID
        //     var sql = "DELETE FROM ingredients WHERE id = @Id";

        //     // Act
        //     using var connection = DatabaseConnection.GetConnection().Create();
        //     connection.Open();
        //     var result = await connection.ExecuteAsync(sql, new { Id = id });

        //     // Assert
        //     Assert.Equal(1, result);  // Should return 1 affected row
        // }
    }
}