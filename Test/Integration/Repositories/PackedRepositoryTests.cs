using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;

using Npgsql;
using Xunit;

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
    public async Task GetAllAsync_Success()
    {
        // Arrange
        List<MealDTO> meals = new List<MealDTO>
        {
            new MealDTO { Id = 1, Name = "Meal 1", Date = "2023-01-01", Template = false, Image_id = null, User_id = 1 },
            new MealDTO { Id = 2, Name = "Meal 2", Date = "2023-01-02", Template = false, Image_id = null, User_id = 2 }
        };
        List<IngredientDTO> ingredients = new List<IngredientDTO>
        {
            new IngredientDTO { Id = 1, Name = "Ingredient 1", User_id = 1, Image_id = null },
            new IngredientDTO { Id = 2, Name = "Ingredient 2", User_id = 2, Image_id = null },
            new IngredientDTO { Id = 3, Name = "Ingredient 3", User_id = 2, Image_id = null }
        };

        var createdIds = await SetupTestEnvironmentAsync(meals, ingredients);

        List<PackedDTO> packed = new List<PackedDTO>
        {
            new PackedDTO { Meal_id = createdIds.MealIds[0], Ingredient_id = createdIds.IngredientIds[0], order_no = 1},
            new PackedDTO { Meal_id = createdIds.MealIds[0], Ingredient_id = createdIds.IngredientIds[1], order_no = 2},
            new PackedDTO { Meal_id = createdIds.MealIds[1], Ingredient_id = createdIds.IngredientIds[2], order_no = 1}
        };
        
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        

        try
        {
            // Act
            var result = await repository.GetAllAsync();

            // Assert
        }
        finally
        {
            await CleanupTestEnvironmentAsync(createdIds);
        }
    }

    // Helper function to set up environment for tests
    private async Task<CreatedIds> SetupTestEnvironmentAsync(List<MealDTO> meals, List<IngredientDTO> ingredients, List<PackedDTO>? packed = null)
    {
        // Open a connection to the database
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        // Lists to store the IDs of inserted meals, ingredients, and packed items
        var meal_ids = new List<int>();
        var ingredient_ids = new List<int>();
        var packed_ids = new List<List<int>>();

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

            int id = await insertCommand.ExecuteNonQueryAsync();
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

            int id = await insertCommand.ExecuteNonQueryAsync();
            ingredient_ids.Add(id);
        }

        // If packed items are provided, insert them into the database
        if (packed != null)
        {
            for (int i = 0; i < packed.Count; i++)
            {
                await using var insertCommand = new NpgsqlCommand(
                    "INSERT INTO packed (meal_id, ingredient_id) " +
                    "VALUES (@meal_id, @ingredient_id)",
                    connection);

                insertCommand.Parameters.AddWithValue("meal_id", packed[i].Meal_id);
                insertCommand.Parameters.AddWithValue("ingredient_id", packed[i].Ingredient_id);

                await insertCommand.ExecuteNonQueryAsync();
                packed_ids.Add(new List<int> { packed[i].Meal_id, packed[i].Ingredient_id });
            }

            var createdIdsWithPacked = new CreatedIds { MealIds = meal_ids, IngredientIds = ingredient_ids, PackedIds = packed_ids };
            return createdIdsWithPacked;
        }

        var createdIdsWithoutPacked = new CreatedIds { MealIds = meal_ids, IngredientIds = ingredient_ids};
        return createdIdsWithoutPacked;
    }

    // Helper function to clean up test environment
    private async Task CleanupTestEnvironmentAsync(CreatedIds createdIds)
    {
        // Gather values from the CreatedIds struct
        var meal_ids = createdIds.MealIds;
        var ingredient_ids = createdIds.IngredientIds;
        var packed_ids = createdIds.PackedIds;

        // Open connection to the database
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        // Delete all meals from ids
        foreach (var meal_id in meal_ids)
        {
            await using var deleteCommand = new NpgsqlCommand("DELETE FROM meals WHERE id = @id", connection);
            deleteCommand.Parameters.AddWithValue("id", meal_id);
            await deleteCommand.ExecuteNonQueryAsync();
        }

        // Delete all ingredients from ids
        foreach (var ingredient_id in ingredient_ids)
        {
            await using var deleteCommand = new NpgsqlCommand("DELETE FROM ingredients WHERE id = @id", connection);
            deleteCommand.Parameters.AddWithValue("id", ingredient_id);
            await deleteCommand.ExecuteNonQueryAsync();
        }

        // If packed_ids is not null, delete all packed items from ids
        if (packed_ids != null)
        {
            foreach (var packed_id in packed_ids)
            {
                await using var deleteCommand = new NpgsqlCommand("DELETE FROM packed WHERE meal_id = @meal_id AND ingredient_id = @ingredient_id", connection);
                deleteCommand.Parameters.AddWithValue("meal_id", packed_id[0]);
                deleteCommand.Parameters.AddWithValue("ingredient_id", packed_id[1]);
                await deleteCommand.ExecuteNonQueryAsync();
            }
        }
    }

    // Struct to hold the IDs of created meals, ingredients, and packed items
    private struct CreatedIds
    {
        public List<int> MealIds { get; set; }
        public List<int> IngredientIds { get; set; }
        public List<List<int>>? PackedIds { get; set; }
    }
}