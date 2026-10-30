using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;
using Xunit;

public class MealsRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly MealsRepository repository;

    public MealsRepositoryTests()
    {
        repository = new MealsRepository(mockExecutor.Object);
    }

    private static bool HasParameter(object? parameters, string name, object? expectedValue)
    {
        if (parameters == null)
            return false;

        var property = parameters.GetType().GetProperty(name);

        if (property == null)
            return false;

        return Equals(property.GetValue(parameters), expectedValue);
    }

    [Fact]
    public async Task GetAllAsync_Success()
    {
        // Arrange
        var meals = new List<MealDTO>
        {
            new() { Id = 1, Name = "Pizza", User_id = 1, Date = "18/11/2022", Template = false },
            new() { Id = 2, Name = "Pasta", User_id = 2, Date = "19/11/2022", Template = false }
        };

        const string sql = "SELECT * FROM meals";

        mockExecutor
            .Setup(e => e.QueryAsync<MealDTO>(sql, It.IsAny<object?>()))
            .ReturnsAsync(meals);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal("Pizza", result[0].Name);
        Assert.Equal(1, result[0].User_id);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("Pasta", result[1].Name);
        Assert.Equal(2, result[1].User_id);

        mockExecutor.Verify(e => e.QueryAsync<MealDTO>(sql, null), Times.Once);
    }

    [Fact]
    public async Task GetAllByUserAndDateAsync_Success()
    {
        // Arrange
        int userId = 1;
        string date = "18/11/2022";

        var meals = new List<MealDTO>
        {
            new() { Id = 1, Name = "Pizza", User_id = userId, Date = date, Template = false },
            new() { Id = 2, Name = "Pasta", User_id = userId, Date = date, Template = false }
        };

        const string sql = "SELECT * FROM meals WHERE user_id = @UserId AND date = @Date";

        mockExecutor
            .Setup(e => e.QueryAsync<MealDTO>(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "UserId", userId) &&
                    HasParameter(p, "Date", date))))
            .ReturnsAsync(meals);

        // Act
        var result = (await repository.GetAllByUserAndDateAsync(userId, date)).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal("Pizza", result[0].Name);
        Assert.Equal(userId, result[0].User_id);
        Assert.Equal(date, result[0].Date);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("Pasta", result[1].Name);
        Assert.Equal(userId, result[1].User_id);
        Assert.Equal(date, result[1].Date);

        mockExecutor.Verify(e => e.QueryAsync<MealDTO>(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "UserId", userId) &&
                HasParameter(p, "Date", date))),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Success()
    {
        // Arrange
        int id = 1;

        var meal = new MealDTO
        {
            Id = id,
            Name = "Pizza",
            User_id = 3,
            Date = "18/11/2022",
            Template = false
        };

        const string sql = "SELECT * FROM meals WHERE id = @Id";

        mockExecutor
            .Setup(e => e.QuerySingleOrDefaultAsync<MealDTO>(
                sql,
                It.Is<object?>(p => HasParameter(p, "Id", id))))
            .ReturnsAsync(meal);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Pizza", result.Name);
        Assert.Equal(3, result.User_id);
        Assert.Equal("18/11/2022", result.Date);

        mockExecutor.Verify(e => e.QuerySingleOrDefaultAsync<MealDTO>(
            sql,
            It.Is<object?>(p => HasParameter(p, "Id", id))),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_MealDoesNotExist()
    {
        // Arrange
        int id = 99;
        const string sql = "SELECT * FROM meals WHERE id = @Id";

        mockExecutor
            .Setup(e => e.QuerySingleOrDefaultAsync<MealDTO>(
                sql,
                It.Is<object?>(p => HasParameter(p, "Id", id))))
            .ReturnsAsync((MealDTO?)null);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        Assert.Null(result);

        mockExecutor.Verify(e => e.QuerySingleOrDefaultAsync<MealDTO>(
            sql,
            It.Is<object?>(p => HasParameter(p, "Id", id))),
            Times.Once);
    }

    [Fact]
    public async Task InsertAsync_Success()
    {
        // Arrange
        int userId = 1;
        int createdId = 5;

        var meal = new MealCreateDTO
        {
            Id = "0",
            Name = "Pizza",
            Date = "18/11/2022",
            Template = false,
            Image_id = null
        };

        const string sql = "INSERT INTO meals (name, user_id, image_id, date, template) VALUES (@Name, @UserId, @ImageId, @Date, @Template) RETURNING id";

        mockExecutor
            .Setup(e => e.QuerySingleAsync<int>(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "Name", meal.Name) &&
                    HasParameter(p, "UserId", userId) &&
                    HasParameter(p, "ImageId", DBNull.Value) &&
                    HasParameter(p, "Date", meal.Date) &&
                    HasParameter(p, "Template", meal.Template))))
            .ReturnsAsync(createdId);

        // Act
        var result = await repository.InsertAsync(meal, userId);

        // Assert
        Assert.Equal(createdId, result);

        mockExecutor.Verify(e => e.QuerySingleAsync<int>(
            sql,
            It.IsAny<object?>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        // Arrange
        var meal = new MealDTO
        {
            Id = 1,
            Name = "Updated Pizza",
            User_id = 2,
            Date = "19/11/2022",
            Template = true,
            Image_id = null
        };

        const string sql = "UPDATE meals SET name = @Name, user_id = @UserId, image_id = @ImageId, date = @Date, template = @Template WHERE id = @Id";

        mockExecutor
            .Setup(e => e.ExecuteAsync(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "Id", meal.Id) &&
                    HasParameter(p, "Name", meal.Name) &&
                    HasParameter(p, "UserId", meal.User_id) &&
                    HasParameter(p, "ImageId", DBNull.Value) &&
                    HasParameter(p, "Date", meal.Date) &&
                    HasParameter(p, "Template", meal.Template))))
            .ReturnsAsync(1);

        // Act
        var result = await repository.UpdateAsync(meal);

        // Assert
        Assert.Equal(1, result);

        mockExecutor.Verify(e => e.ExecuteAsync(sql, It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        // Arrange
        int id = 1;
        const string sql = "DELETE FROM meals WHERE id = @Id";

        mockExecutor
            .Setup(e => e.ExecuteAsync(
                sql,
                It.Is<object?>(p => HasParameter(p, "Id", id))))
            .ReturnsAsync(1);

        // Act
        var result = await repository.DeleteAsync(id);

        // Assert
        Assert.Equal(1, result);

        mockExecutor.Verify(e => e.ExecuteAsync(
            sql,
            It.Is<object?>(p => HasParameter(p, "Id", id))),
            Times.Once);
    }

    [Fact]
    public async Task GetAllTemplatesByUserAsync_Success()
    {
        // Arrange
        int userId = 1;

        var meals = new List<MealDTO>
        {
            new() { Id = 1, Name = "Pizza", User_id = userId, Date = "18/11/2022", Template = true },
            new() { Id = 2, Name = "Pasta", User_id = userId, Date = "19/11/2022", Template = true }
        };

        const string sql = "SELECT * FROM meals WHERE template = true AND user_id = @UserId";

        mockExecutor
            .Setup(e => e.QueryAsync<MealDTO>(
                sql,
                It.Is<object?>(p => HasParameter(p, "UserId", userId))))
            .ReturnsAsync(meals);

        // Act
        var result = (await repository.GetAllTemplatesByUserAsync(userId)).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal("Pizza", result[0].Name);
        Assert.Equal(userId, result[0].User_id);
        Assert.True(result[0].Template);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("Pasta", result[1].Name);
        Assert.Equal(userId, result[1].User_id);
        Assert.True(result[1].Template);

        mockExecutor.Verify(e => e.QueryAsync<MealDTO>(
            sql,
            It.Is<object?>(p => HasParameter(p, "UserId", userId))),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTemplateStatusAsync_Success()
    {
        // Arrange
        int id = 1;
        bool template = true;

        const string sql = "UPDATE meals SET template = @template WHERE id = @Id";

        mockExecutor
            .Setup(e => e.ExecuteAsync(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "Id", id) &&
                    HasParameter(p, "Template", template))))
            .ReturnsAsync(1);

        // Act
        var result = await repository.UpdateTemplateStatusAsync(id, template);

        // Assert
        Assert.Equal(1, result);

        mockExecutor.Verify(e => e.ExecuteAsync(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "Id", id) &&
                HasParameter(p, "Template", template))),
            Times.Once);
    }
}