using Moq;
using Npgsql;
using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;

namespace testing 
{
    public class IngredientRepositoryTests
    {
        // private readonly Mock<PostgreSQLConnectionFactory> _mockConnectionFactory;
        // private readonly Mock<NpgsqlConnection> _mockDbConnection;
        // private readonly IngredientRepository _repository;

        // public IngredientRepositoryTests() {
        //     // set up the mocks
        //     _mockConnectionFactory = new Mock<PostgreSQLConnectionFactory>();
        //     _mockDbConnection = new Mock<NpgsqlConnection>();

        //     // configure the connection factory mock to return the mocked connection
        //     _mockConnectionFactory.Setup(f => f.Create()).Returns(_mockDbConnection.Object);
            
        //     // Create the repository instance with the mock connection factory
        //     _repository = new IngredientRepository(_mockConnectionFactory.Object);
        // }
        
        // [Fact]
        // public async Task GetAllAsync_ReturnsAllIngredients()
        // {
        //     // Arrange
        //     var expectedIngredients = new List<Ingredient> 
        //     {
        //         new Ingredient { Id = 1, Name = "Tomato", User_ref = "user1", Image_ref = "image1" },
        //         new Ingredient { Id = 2, Name = "Cheese", User_ref = "user2", Image_ref = "image2" }
        //     };
        //     // Setup mock Dapper Query
        //     _mockDbConnection.Setup(db => await db.QueryAsync<Ingredient>(
        //         It.IsAny<string>(),
        //         null,
        //         null,
        //         null,
        //         null))
        //         .ReturnsAsync(expectedIngredients);

        //     // Act
        //     var actual = await _repository.GetAllAsync();

        //     // Assert
        //     Assert.Equal(expectedIngredients.Count, actual.Count());
        //     Assert.Equal(expectedIngredients, actual);
        // }

        // [Theory]
        // [InlineData(1, "Tomato")]
        // [InlineData(2, "Cheese")]
        // public async Task GetByIdAsync_ReturnsCorrectIngredient(int id, string expectedName) {
        //     // Arrange
        //     var expectedIngredient = new Ingredient { Id = id, Name = expectedName, User_ref = "user_ref", Image_ref = "image_ref" };

        //     _mockDbConnection.Setup(db => await db.QuerySingleOrDefaultAsync<Ingredient>(
        //         It.IsAny<string>(),
        //         It.IsAny<object>(),
        //         null,
        //         null,
        //         null))
        //         .ReturnsAsync(expectedIngredient);

            
        //     // Act
        //     var actual = await _repository.GetByIdAsync(id);

        //     // Assert
        //     Assert.NotNull(actual);
        //     Assert.Equal(expectedName, actual.Name);
        // }

        // [Fact]
        // public async Task InsertAsync_InsertsNewIngredient()
        // {
        //     // Arrange
        //     var newIngredient = new Ingredient { Name = "Lettuce", User_ref = "user3", Image_ref = "image3" };

        //      _mockDbConnection.Setup(db => await db.ExecuteAsync(
        //         It.IsAny<string>(),
        //         It.IsAny<object>(),
        //         null,
        //         null,
        //         null))
        //         .ReturnsAsync(1);

        //     // Act
        //     var result = await _repository.InsertAsync(newIngredient);

        //     // Assert
        //     Assert.Equal(1, result);
        // }

        // [Fact]
        // public async Task UpdateAsync_UpdatesExistingIngredient()
        // {
        //     // Arrange
        //     var updatedIngredient = new Ingredient { Name = "Lettuce", User_ref = "user3", Image_ref = "image3" };
        //     int id = 1;

        //     _mockDbConnection.Setup(db => await db.ExecuteAsync(
        //         It.IsAny<string>(),
        //         It.IsAny<object>(),
        //         null,
        //         null,
        //         null))
        //         .ReturnsAsync(1); // Assume 1 row was affected

        //     // Act
        //     var result = await _repository.UpdateAsync(updatedIngredient, id);

        //     // Assert
        //     Assert.Equal(1, result);
        // }

        // [Fact]
        // public async Task DeleteAsync_DeletesIngredient()
        // {
        //     // Arrange
        //     int id = 1;

        //     _mockDbConnection.Setup(db => await db.ExecuteAsync(
        //         It.IsAny<string>(),
        //         It.IsAny<object>(),
        //         null,
        //         null,
        //         null))
        //         .ReturnsAsync(1); // Assume 1 row was affected

        //     // Act
        //     var result = await _repository.DeleteAsync(id);

        //     // Assert
        //     Assert.Equal(1, result);
        // }
    
    }
}
