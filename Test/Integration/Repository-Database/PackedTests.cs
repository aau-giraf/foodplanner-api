using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Npgsql;

namespace Test.Integration.Repositories;

[Collection("IntegrationDatabase")]
public class PackedTests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly PackedRepository repository;
    public PackedTests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;
        repository = new PackedRepository(new DapperExecutor(connectionFactory));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByMealIdAsync()
    {
        // Arrange
        List<Meal> meals = new List<Meal>
        {
            new Meal { Id = 1,Name = "Meal 1", Date = DateOnly.Parse("2023/01/01"), Template = false, Image_id = null, User_id = 1 },
            new Meal { Id = 2, Name = "Meal 2", Date = DateOnly.Parse("2023/01/02"), Template = false, Image_id = null, User_id = 2 }
        };
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
            new Ingredient { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
            new Ingredient { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
        };
        List<Packed> packed = new List<Packed>
        {
            new Packed { Meal_id = 0, Ingredient_id = 0, order_no = 2},
            new Packed { Meal_id = 0, Ingredient_id = 1, order_no = 1},
            new Packed { Meal_id = 1, Ingredient_id = 2, order_no = 1}
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

    [Fact]
[Trait("Category", "Integration")]
public async Task InsertAsync()
{
    // Arrange
    List<Meal> meals = new()
    {
        new Meal { Id = 1, Name = "Meal 1", Date = DateOnly.Parse("2023/01/01"), Template = false, Image_id = null, User_id = 1 },
        new Meal { Id = 2, Name = "Meal 2", Date = DateOnly.Parse("2023/01/02"), Template = false, Image_id = null, User_id = 2 }
    };

    List<Ingredient> ingredients = new()
    {
        new Ingredient { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
        new Ingredient { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
        new Ingredient { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
    };

    try
    {
        var createdIds = await SetupTestEnvironmentAsync(meals, ingredients);

        Packed packed = new()
        {
            Meal_id = createdIds[0][0],
            Ingredient_id = createdIds[1][0],
            order_no = 1
        };

        // Act
        var result = await repository.InsertAsync(packed);

        // Assert
        Assert.Equal(1, result);

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT meal_id, ingredient_id, order_no FROM packed " +
            "WHERE meal_id = @mealId AND ingredient_id = @ingredientId",
            connection);

        command.Parameters.AddWithValue("mealId", packed.Meal_id);
        command.Parameters.AddWithValue("ingredientId", packed.Ingredient_id);

        await using var reader = await command.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal(packed.Meal_id, reader.GetInt32(0));
        Assert.Equal(packed.Ingredient_id, reader.GetInt32(1));
        Assert.Equal(packed.order_no, reader.GetInt32(2));

        Assert.False(await reader.ReadAsync());
    }
    finally
    {
        await CleanupTestEnvironmentAsync();
    }
}

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync()
    {
        // Arrange
        List<Meal> meals = new List<Meal>
        {
            new Meal { Id = 1,Name = "Meal 1", Date = DateOnly.Parse("2023/01/01"), Template = false, Image_id = null, User_id = 1 },
            new Meal { Id = 2, Name = "Meal 2", Date = DateOnly.Parse("2023/01/02"), Template = false, Image_id = null, User_id = 2 }
        };
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
            new Ingredient { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
            new Ingredient { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
        };
        List<Packed> packed = new List<Packed>
        {
            new Packed { Meal_id = 0, Ingredient_id = 0, order_no = 2},
            new Packed { Meal_id = 0, Ingredient_id = 1, order_no = 1},
            new Packed { Meal_id = 1, Ingredient_id = 2, order_no = 1}
        };

        try
        {
            var createdIds = await SetupTestEnvironmentAsync(meals, ingredients, packed);
            Packed oldPacked = new Packed { Meal_id = createdIds[0][1], Ingredient_id = createdIds[1][2], order_no = 1 };
            Packed newPacked = new Packed { Meal_id = createdIds[0][0], Ingredient_id = createdIds[1][2], order_no = 0 };

            // Act
            var result = await repository.UpdateAsync(oldPacked, newPacked);

            // Assert
            Assert.Equal(1, result);
            var packedFromMeal = await repository.GetAllByMealIdAsync(createdIds[0][0]);
            Assert.Contains(packedFromMeal, item =>
                item.Meal_id == newPacked.Meal_id &&
                item.Ingredient_id == newPacked.Ingredient_id &&
                item.order_no == newPacked.order_no);
            var resultList = packedFromMeal.ToList();
            Assert.Equal(3, resultList.Count);
        }
        finally
        {
            await CleanupTestEnvironmentAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync()
    {
        // Arrange
        List<Meal> meals = new List<Meal>
        {
            new Meal { Id = 1,Name = "Meal 1", Date = DateOnly.Parse("2023/01/01"), Template = false, Image_id = null, User_id = 1 },
            new Meal { Id = 2, Name = "Meal 2", Date = DateOnly.Parse("2023/01/02"), Template = false, Image_id = null, User_id = 2 }
        };
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
            new Ingredient { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
            new Ingredient { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
        };
        List<Packed> packed = new List<Packed>
        {
            new Packed { Meal_id = 0, Ingredient_id = 0, order_no = 2},
            new Packed { Meal_id = 0, Ingredient_id = 1, order_no = 1},
            new Packed { Meal_id = 1, Ingredient_id = 2, order_no = 1}
        };

        try
        {
            var createdIds = await SetupTestEnvironmentAsync(meals, ingredients, packed);
            Packed toDelete = new Packed { Meal_id = createdIds[0][0], Ingredient_id = createdIds[1][1], order_no = 1 };

            // Act
            var result = await repository.DeleteAsync(toDelete);

            // Assert
            Assert.Equal(1, result);
            var packedFromMeal = await repository.GetAllByMealIdAsync(createdIds[0][0]);
            Assert.DoesNotContain(packedFromMeal, item =>
                item.Meal_id == toDelete.Meal_id &&
                item.Ingredient_id == toDelete.Ingredient_id &&
                item.order_no == toDelete.order_no);
            var resultList = packedFromMeal.ToList();
            Assert.Single(resultList);
        }
        finally
        {
            await CleanupTestEnvironmentAsync();
        }
    }

    // Helper function to set up environment for tests
    private async Task<List<List<int>>> SetupTestEnvironmentAsync(List<Meal> meals, List<Ingredient> ingredients, List<Packed>? packed = null)
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