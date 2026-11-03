using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Npgsql;
using Test.Integration;

[Collection("IntegrationDatabase")]
public class PackedRepositorytests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly PackedRepository repository;
    public PackedRepositorytests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;
        repository = new PackedRepository(new DapperExecutor(connectionFactory));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync()
    {
        // Arrange
        List<MealDTO> meals = new List<MealDTO>
        {
            new MealDTO { Id = 1,Name = "Meal 1", Date = "2023-01-01", Template = false, Image_id = null, User_id = 1 },
            new MealDTO { Id = 2, Name = "Meal 2", Date = "2023-01-02", Template = false, Image_id = null, User_id = 2 }
        };
        List<IngredientDTO> ingredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
            new IngredientDTO { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
            new IngredientDTO { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
        };
        List<PackedDTO> packed = new List<PackedDTO>
        {
            new PackedDTO { Meal_id = 0, Ingredient_id = 0, order_no = 2},
            new PackedDTO { Meal_id = 0, Ingredient_id = 1, order_no = 1},
            new PackedDTO { Meal_id = 1, Ingredient_id = 2, order_no = 1}
        };

        try
        {
            var createdIds = await SetupTestEnvironmentAsync(meals, ingredients, packed);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Contains(resultList, item =>
                item.Meal_id == createdIds[0][0] &&
                item.Ingredient_id == createdIds[1][1] &&
                item.order_no == 1);
            Assert.Contains(resultList, item =>
                item.Meal_id == createdIds[0][0] &&
                item.Ingredient_id == createdIds[1][0] &&
                item.order_no == 2);
            Assert.Contains(resultList, item =>
                item.Meal_id == createdIds[0][1] &&
                item.Ingredient_id == createdIds[1][2] &&
                item.order_no == 1);
        }
        finally
        {
            await CleanupTestEnvironmentAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByMealIdAsync()
    {
        // Arrange
        List<MealDTO> meals = new List<MealDTO>
        {
            new MealDTO { Id = 1,Name = "Meal 1", Date = "2023-01-01", Template = false, Image_id = null, User_id = 1 },
            new MealDTO { Id = 2, Name = "Meal 2", Date = "2023-01-02", Template = false, Image_id = null, User_id = 2 }
        };
        List<IngredientDTO> ingredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
            new IngredientDTO { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
            new IngredientDTO { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
        };
        List<PackedDTO> packed = new List<PackedDTO>
        {
            new PackedDTO { Meal_id = 0, Ingredient_id = 0, order_no = 2},
            new PackedDTO { Meal_id = 0, Ingredient_id = 1, order_no = 1},
            new PackedDTO { Meal_id = 1, Ingredient_id = 2, order_no = 1}
        };

        try
        {
            var createdIds = await SetupTestEnvironmentAsync(meals, ingredients, packed);

            // Act
            var result = await repository.GetAllByMealIdAsync(createdIds[0][0]);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, item =>
                item.Meal_id == createdIds[0][0] &&
                item.Ingredient_id == createdIds[1][1] &&
                item.order_no == 1);
            Assert.Contains(resultList, item =>
                item.Meal_id == createdIds[0][0] &&
                item.Ingredient_id == createdIds[1][0] &&
                item.order_no == 2);
        }
        finally
        {
            await CleanupTestEnvironmentAsync();
        }
    }

    // Helper function to set up environment for tests
    private async Task<List<List<int>>> SetupTestEnvironmentAsync(List<MealDTO> meals, List<IngredientDTO> ingredients, List<PackedDTO>? packed = null)
    {
        // Open a connection to the database
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        // Lists to store the IDs of inserted meals, ingredients, and packed items
        var meal_ids = new List<int>();
        var ingredient_ids = new List<int>();

        // Insert all meals into the database
        for (int i = 0; i < meals.Count; i++)
        {
            await using var insertCommand = new NpgsqlCommand(
                "INSERT INTO meals (name, date, image_id, user_id, template) " +
                "VALUES (@name, @date, @image_id, @user_id, @template) RETURNING id",
                connection);

            insertCommand.Parameters.AddWithValue("name", meals[i].Name);
            insertCommand.Parameters.AddWithValue("date", meals[i].Date);
            insertCommand.Parameters.AddWithValue("image_id", meals[i].Image_id ?? (object)DBNull.Value);
            insertCommand.Parameters.AddWithValue("user_id", meals[i].User_id);
            insertCommand.Parameters.AddWithValue("template", meals[i].Template);

            int id = (int)(await insertCommand.ExecuteScalarAsync())!;
            meal_ids.Add(id);
        }

        // Insert all ingredients into the database
        for (int i = 0; i < ingredients.Count; i++)
        {
            await using var insertCommand = new NpgsqlCommand(
                "INSERT INTO ingredients (name, user_id, image_id) " +
                "VALUES (@name, @user_id, @image_id) RETURNING id",
                connection);

            insertCommand.Parameters.AddWithValue("name", ingredients[i].Name);
            insertCommand.Parameters.AddWithValue("user_id", ingredients[i].User_id);
            insertCommand.Parameters.AddWithValue("image_id", ingredients[i].Image_id ?? (object)DBNull.Value);

            int id = (int)(await insertCommand.ExecuteScalarAsync())!;
            ingredient_ids.Add(id);
        }

        // If packed items are provided, insert them into the database
        if (packed != null)
        {
            for (int i = 0; i < packed.Count; i++)
            {
                await using var insertCommand = new NpgsqlCommand(
                    "INSERT INTO packed (meal_id, ingredient_id, order_no) " +
                    "VALUES (@meal_id, @ingredient_id, @order_no)",
                    connection);

                insertCommand.Parameters.AddWithValue("meal_id", meal_ids[packed[i].Meal_id]);
                insertCommand.Parameters.AddWithValue("ingredient_id", ingredient_ids[packed[i].Ingredient_id]);
                insertCommand.Parameters.AddWithValue("order_no", packed[i].order_no);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }
        var createdIds = new List<List<int>> {meal_ids, ingredient_ids};
        return createdIds;
    }

    // Helper function to clean up test environment
    private async Task CleanupTestEnvironmentAsync()
    {
        // Open connection to the database
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        // Delete packed, meals, and ingredients from the database
        await using (var command = new NpgsqlCommand("DELETE FROM packed", connection))
        {
            await command.ExecuteNonQueryAsync();
        }
        await using (var command = new NpgsqlCommand("DELETE FROM meals", connection))
        {
            await command.ExecuteNonQueryAsync();
        }
        await using (var command = new NpgsqlCommand("DELETE FROM ingredients", connection))
        {
            await command.ExecuteNonQueryAsync();
        }
    }
}