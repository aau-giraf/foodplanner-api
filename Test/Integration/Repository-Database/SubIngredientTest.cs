using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Npgsql;

namespace Test.Integration.Repositories;

[Collection("IntegrationDatabase")]
public class SubIngredientTests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly SubIngredientRepository repository;

    private readonly List<int> createdIngredientIds = new();

    public SubIngredientTests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;

        repository = new SubIngredientRepository(
            new DapperExecutor(connectionFactory));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_ShouldReturnInsertedSubIngredients()
    {
        try
        {
            // Arrange
            int parentId = await InsertTestIngredientAsync();
            int childId1 = await InsertTestIngredientAsync();
            int childId2 = await InsertTestIngredientAsync();

            await InsertTestSubIngredientAsync(parentId, childId1, 1);
            await InsertTestSubIngredientAsync(parentId, childId2, 2);

            // Act
            var result = (await repository.GetAllAsync()).ToList();

            // Assert
            var first = Assert.Single(result.Where(s =>
                s.Parent_id == parentId &&
                s.Child_id == childId1));

            Assert.Equal(1, first.order_no);

            var second = Assert.Single(result.Where(s =>
                s.Parent_id == parentId &&
                s.Child_id == childId2));

            Assert.Equal(2, second.order_no);
        }
        finally
        {
            // Cleanup
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByParentIdAsync_ShouldReturnOnlyMatchingSubIngredients()
    {
        try
        {
            // Arrange
            int parentId1 = await InsertTestIngredientAsync();
            int parentId2 = await InsertTestIngredientAsync();

            int childId1 = await InsertTestIngredientAsync();
            int childId2 = await InsertTestIngredientAsync();

            await InsertTestSubIngredientAsync(parentId1, childId1, 1);
            await InsertTestSubIngredientAsync(parentId1, childId2, 2);

            await InsertTestSubIngredientAsync(parentId2, childId1, 3);

            // Act
            var result = (await repository
                .GetAllByParentIdAsync(parentId1)).ToList();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Contains(result, s =>
                s.Parent_id == parentId1 &&
                s.Child_id == childId1 &&
                s.order_no == 1);

            Assert.Contains(result, s =>
                s.Parent_id == parentId1 &&
                s.Child_id == childId2 &&
                s.order_no == 2);

            Assert.DoesNotContain(result, s =>
                s.Parent_id == parentId2);

            Assert.All(result, s =>
                Assert.Equal(parentId1, s.Parent_id));
        }
        finally
        {
            // Cleanup
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InsertAsync_ShouldPersistCorrectSubIngredientData()
    {
        try
        {
            // Arrange
            int parentId = await InsertTestIngredientAsync();
            int childId = await InsertTestIngredientAsync();

            var subIngredient = new SubIngredientDTO
            {
                Parent_id = parentId,
                Child_id = childId,
                order_no = 1
            };

            // Act
            int affectedRows = await repository.InsertAsync(subIngredient);

            // Assert
            Assert.Equal(1, affectedRows);

            var stored = await GetStoredSubIngredientAsync(
                parentId, childId);

            Assert.NotNull(stored);

            Assert.Equal(parentId, stored.Value.ParentId);
            Assert.Equal(childId, stored.Value.ChildId);
            Assert.Equal(1, stored.Value.OrderNo);
        }
        finally
        {
            // Cleanup
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_ShouldPersistCorrectSubIngredientData()
    {
        try
        {
            // Arrange
            int oldParentId = await InsertTestIngredientAsync();
            int oldChildId = await InsertTestIngredientAsync();

            int newParentId = await InsertTestIngredientAsync();
            int newChildId = await InsertTestIngredientAsync();

            await InsertTestSubIngredientAsync(
                oldParentId, oldChildId, 1);

            // Another relationship that should remain unchanged
            await InsertTestSubIngredientAsync(
                oldParentId, newChildId, 2);

            var oldSubIngredient = new SubIngredientDTO
            {
                Parent_id = oldParentId,
                Child_id = oldChildId,
                order_no = 1
            };

            var updatedSubIngredient = new SubIngredientDTO
            {
                Parent_id = newParentId,
                Child_id = newChildId,
                order_no = 5
            };

            // Act
            int affectedRows = await repository.UpdateAsync(
                oldSubIngredient,
                updatedSubIngredient);

            // Assert
            Assert.Equal(1, affectedRows);

            var oldRecord = await GetStoredSubIngredientAsync(
                oldParentId, oldChildId);

            var updatedRecord = await GetStoredSubIngredientAsync(
                newParentId, newChildId);

            var unchangedRecord = await GetStoredSubIngredientAsync(
                oldParentId, newChildId);

            Assert.Null(oldRecord);

            Assert.NotNull(updatedRecord);

            Assert.Equal(newParentId, updatedRecord.Value.ParentId);
            Assert.Equal(newChildId, updatedRecord.Value.ChildId);
            Assert.Equal(5, updatedRecord.Value.OrderNo);

            Assert.NotNull(unchangedRecord);
            Assert.Equal(2, unchangedRecord.Value.OrderNo);
        }
        finally
        {
            // Cleanup
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByParentIdAsync_ShouldReturnSubIngredientsInOrder()
    {
        try
        {
            // Arrange
            int parentId = await InsertTestIngredientAsync();
            int childId1 = await InsertTestIngredientAsync();
            int childId2 = await InsertTestIngredientAsync();
            int childId3 = await InsertTestIngredientAsync();

            // Insert the relationships deliberately out of order.
            await InsertTestSubIngredientAsync(parentId, childId1, 3);
            await InsertTestSubIngredientAsync(parentId, childId2, 1);
            await InsertTestSubIngredientAsync(parentId, childId3, 2);

            // Act
            var result = (await repository.GetAllByParentIdAsync(parentId)).ToList();

            // Assert
            Assert.Equal(3, result.Count);

            Assert.Equal(childId2, result[0].Child_id);
            Assert.Equal(1, result[0].order_no);

            Assert.Equal(childId3, result[1].Child_id);
            Assert.Equal(2, result[1].order_no);

            Assert.Equal(childId1, result[2].Child_id);
            Assert.Equal(3, result[2].order_no);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ShouldDeleteOnlySpecifiedSubIngredient()
    {
        try
        {
            // Arrange
            int parentId = await InsertTestIngredientAsync();
            int childId1 = await InsertTestIngredientAsync();
            int childId2 = await InsertTestIngredientAsync();

            await InsertTestSubIngredientAsync(parentId, childId1, 1);
            await InsertTestSubIngredientAsync(parentId, childId2, 2);

            var toDelete = new SubIngredientDTO
            {
                Parent_id = parentId,
                Child_id = childId1,
                order_no = 1
            };

            // Act
            int affectedRows = await repository.DeleteAsync(toDelete);

            // Assert
            Assert.Equal(1, affectedRows);
            Assert.Null(await GetStoredSubIngredientAsync(parentId, childId1));

            var remaining = await GetStoredSubIngredientAsync(parentId, childId2);
            Assert.NotNull(remaining);
            Assert.Equal(parentId, remaining.Value.ParentId);
            Assert.Equal(childId2, remaining.Value.ChildId);
            Assert.Equal(2, remaining.Value.OrderNo);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    // Creates an ingredient to satisfy the foreign key constraints
    private async Task<int> InsertTestIngredientAsync()
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "INSERT INTO ingredients (name, user_id, image_id) " +
            "VALUES (@name, @userId, NULL) RETURNING id",
            connection);

        command.Parameters.AddWithValue(
            "name", $"TestIngredient_{Guid.NewGuid():N}");

        command.Parameters.AddWithValue("userId", 12345);

        int id = (int)(await command.ExecuteScalarAsync())!;

        createdIngredientIds.Add(id);

        return id;
    }

    // Creates a subingredient relationship directly in PostgreSQL
    private async Task InsertTestSubIngredientAsync(int parentId, int childId, int orderNo)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "INSERT INTO subingredient (parent_id, child_id, order_no) " +
            "VALUES (@parentId, @childId, @orderNo)",
            connection);

        command.Parameters.AddWithValue("parentId", parentId);
        command.Parameters.AddWithValue("childId", childId);
        command.Parameters.AddWithValue("orderNo", orderNo);

        await command.ExecuteNonQueryAsync();
    }

    // Reads the actual persisted relationship
    private async Task<(int ParentId, int ChildId,int OrderNo)?> GetStoredSubIngredientAsync(int parentId, int childId)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT parent_id, child_id, order_no " +
            "FROM subingredient " +
            "WHERE parent_id = @parentId AND child_id = @childId",
            connection);

        command.Parameters.AddWithValue("parentId", parentId);
        command.Parameters.AddWithValue("childId", childId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return (
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetInt32(2)
        );
    }

    // Removes relationships before deleting their ingredients
    private async Task CleanupAsync()
    {
        if (createdIngredientIds.Count == 0)
            return;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        int[] ids = createdIngredientIds.ToArray();

        // Delete relationships first because of foreign keys
        await using (var command = new NpgsqlCommand(
            "DELETE FROM subingredient " +
            "WHERE parent_id = ANY(@ids) OR child_id = ANY(@ids)",
            connection))
        {
            command.Parameters.AddWithValue("ids", ids);

            await command.ExecuteNonQueryAsync();
        }

        // Delete the ingredients created for testing
        await using (var command = new NpgsqlCommand(
            "DELETE FROM ingredients WHERE id = ANY(@ids)",
            connection))
        {
            command.Parameters.AddWithValue("ids", ids);

            await command.ExecuteNonQueryAsync();
        }

        createdIngredientIds.Clear();
    }
}