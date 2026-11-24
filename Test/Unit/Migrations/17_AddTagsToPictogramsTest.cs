using System.Data;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FoodplannerDataAccessSql.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NuGet.Frameworks;
using Xunit;

namespace Test.Unit.MigrationTests;

public class AddTagsToPictogramsTests
{
    // Capture FluentMigrator instructions without executing them against a database.
    private static List<IMigrationExpression> GetExpressions(AddTagsToPictograms migration, bool up)
    {
        var expressions = new List<IMigrationExpression>();
        using var services = new ServiceCollection().BuildServiceProvider();
        var context = new Mock<IMigrationContext>();

        context.SetupGet(c => c.Expressions).Returns(expressions);
        context.SetupGet(c => c.ServiceProvider).Returns(services);

        if (up)
            migration.GetUpExpressions(context.Object);
        else
            migration.GetDownExpressions(context.Object);

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
    [Trait("Category", "Unit")]
    public void Up_CreatesCategoriesTable()
    {
        // Arrange
        var migration = new AddTagsToPictograms(); 
        var expectedColumns = new[] {"id", "name", "main"};
        
        // Act
        var expressions = GetExpressions(migration, up:true);
        var tables = expressions.OfType<CreateTableExpression>()
            .Where(e=> e.TableName == "categories")
            .ToList(); 

        // Asset
        var table = Assert.Single(tables); 
        Assert.Equal(expectedColumns, table.Columns.Select(c=>c.Name));
    }

    // Creates Tags Table

    [Fact]
    [Trait("Category", "Unit")]
    public void Up_CreateTagsTable()
    {
         // Arrange
        var migration = new AddTagsToPictograms(); 
        var expectedColumns = new[] {"ingredient_id", "category_id"};
        
        // Act
        var expressions = GetExpressions(migration, up:true);
        var tables = expressions.OfType<CreateTableExpression>()
            .Where(e=> e.TableName == "tags")
            .ToList(); 

        // Asset
        var table = Assert.Single(tables); 
        Assert.Equal(expectedColumns, table.Columns.Select(c=>c.Name));
    
    }

     // Adds Primary keys 
    [Fact]
    [Trait("Category", "Unit")]
    public void Up_CreateTagCompositeKeyAndTwoForeignKeys()
    {
        // Arrange
        var migration = new AddTagsToPictograms();
        var expectedKeyColumns = new[] {"category_id", "ingredient_id"};

        // Act
        var expressions = GetExpressions(migration, up: true);
        var keys = expressions.OfType<CreateConstraintExpression>()
            .Where(e => e.Constraint.ConstraintName == "pk_categories_ingredients")
            .ToList();
        var foreignKeys = expressions.OfType<CreateForeignKeyExpression>().ToList();

        // Assert
        var key = Assert.Single(keys);
         Assert.True(key.Constraint.IsPrimaryKeyConstraint);
        Assert.Equal("tags", key.Constraint.TableName);
        Assert.Equal(expectedKeyColumns, key.Constraint.Columns.OrderBy(c => c));

        Assert.Equal(2, foreignKeys.Count);
        Assert.Contains(foreignKeys, e => e.ForeignKey.Name == "fk_ingredient_id"
            && e.ForeignKey.ForeignTable == "tags"
            && e.ForeignKey.PrimaryTable == "ingredients"
            && e.ForeignKey.ForeignColumns.SequenceEqual(new[] { "ingredient_id" })
            && e.ForeignKey.PrimaryColumns.SequenceEqual(new[] { "id" }));
        Assert.Contains(foreignKeys, e => e.ForeignKey.Name == "fk_category_id"
            && e.ForeignKey.ForeignTable == "tags"
            && e.ForeignKey.PrimaryTable == "categories"
            && e.ForeignKey.ForeignColumns.SequenceEqual(new[] { "category_id" })
            && e.ForeignKey.PrimaryColumns.SequenceEqual(new[] { "id" }));
    }

    // Adds column to ingredients 
    [Fact]
    [Trait("Category", "Unit")]
    public void Up_AddsIsStockImgColumnToIngredients()
    {
    // Arrange
    var migration = new AddTagsToPictograms();

    // Act
    var expressions = GetExpressions(migration, up:true);

    var column = Assert.Single(
        expressions.OfType<CreateColumnExpression>()
            .Where(x => x.Column.Name == "is_stock_img"));

    // Assert
    Assert.Equal("ingredients", column.TableName);
    Assert.Equal("is_stock_img", column.Column.Name);
    Assert.Equal(DbType.Boolean, column.Column.Type);
    Assert.False(column.Column.IsNullable);
    Assert.Equal(false, column.Column.DefaultValue);
    }

    // Down
    [Fact]
    [Trait("Category", "Unit")]
    public void Down_RemovesAllCoreTablesInDependencyOrder()
    {
        // Arrange
        var migration = new AddTagsToPictograms();
        var expectedTables = new[]
        {
            "tags", "categories"
        };
        var expectedColumn = "is_stock_img";

        // Act
        var expressions = GetExpressions(migration, up: false);
        var deleted = expressions.OfType<DeleteTableExpression>()
            .Select(t => t.TableName)
            .ToList();
        var deletedColumn = Assert.Single(expressions.OfType<DeleteColumnExpression>());
        
            // Assert
        Assert.Equal(3, expressions.Count);
        Assert.Equal(expectedTables, deleted);
        Assert.Equal(expectedColumn, Assert.Single(deletedColumn.ColumnNames));
    }
}