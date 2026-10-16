using System.Data;
using System.Reflection;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FoodplannerDataAccessSql.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Test.MigrationTests;

public class MigrateUserDataTests
{
    // Collect instructions produced by Up() without executing the database operation.
    private static List<IMigrationExpression> GetUpExpressions(MigrateUserData migration)
    {
        var expressions = new List<IMigrationExpression>();
        using var services = new ServiceCollection().BuildServiceProvider();
        var context = new Mock<IMigrationContext>();

        context.SetupGet(c => c.Expressions).Returns(expressions);
        context.SetupGet(c => c.ServiceProvider).Returns(services);

        migration.GetUpExpressions(context.Object);
        return expressions;
    }

    // Call the existing private SQL helper using mocked ADO.NET interfaces.
    // This does not connect to PostgreSQL or change production code.
    private static void InvokeUpdateSequence(
        IDbConnection connection,
        IDbTransaction transaction,
        string table,
        string idColumn)
    {
        var method = typeof(MigrateUserData).GetMethod(
            "UpdateSequence",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        method.Invoke(null, new object[] { connection, transaction, table, idColumn });
    }

    [Fact]
    public void Up_RegistersDatabaseOperation()
    {
        // Arrange
        var migration = new MigrateUserData(
            new DataMigrationSource("Host=localhost;Database=source_test"));

        // Act
        var expressions = GetUpExpressions(migration);

        // Assert
        var operation = Assert.Single(expressions);
        var databaseOperation = Assert.IsType<PerformDBOperationExpression>(operation);
        Assert.NotNull(databaseOperation.Operation);
    }

    [Theory]
    [InlineData("users", "id")]
    [InlineData("classroom", "class_id")]
    public void UpdateSequence_UsesCorrectTableAndIdColumn(
        string table,
        string idColumn)
    {
        // Arrange
        var command = new Mock<IDbCommand>();
        command.SetupAllProperties();
        var connection = new Mock<IDbConnection>();
        connection.Setup(c => c.CreateCommand()).Returns(command.Object);
        var transaction = new Mock<IDbTransaction>();

        // Act
        InvokeUpdateSequence(connection.Object, transaction.Object, table, idColumn);

        // Assert
        var sql = command.Object.CommandText;
        Assert.Contains($"pg_get_serial_sequence('{table}', '{idColumn}')", sql);
        Assert.Contains($"MAX({idColumn})", sql);
        Assert.Contains($"FROM {table};", sql);
        Assert.Contains("setval(", sql);
        Assert.Contains("MAX(", sql);
        Assert.Contains("IS NOT NULL", sql);
    }

    [Fact]
    public void UpdateSequence_ExecutesCommandUsingGivenTransaction()
    {
        // Arrange
        var command = new Mock<IDbCommand>();
        command.SetupAllProperties();
        var connection = new Mock<IDbConnection>();
        connection.Setup(c => c.CreateCommand()).Returns(command.Object);
        var transaction = new Mock<IDbTransaction>();

        // Act
        InvokeUpdateSequence(connection.Object, transaction.Object, "users", "id");

        // Assert
        Assert.Same(transaction.Object, command.Object.Transaction);
        connection.Verify(c => c.CreateCommand(), Times.Once);
        command.Verify(c => c.ExecuteNonQuery(), Times.Once);
        command.Verify(c => c.Dispose(), Times.Once);
    }

    [Fact]
    public void Down_ThrowsNotSupportedException()
    {
        // Arrange
        var migration = new MigrateUserData(
            new DataMigrationSource("Host=localhost;Database=source_test"));

        // Act
        var exception = Assert.Throws<NotSupportedException>(() => migration.Down());

        // Assert
        Assert.Equal(
            "User data migration cannot be automatically rolled back.",
            exception.Message);
    }
}
