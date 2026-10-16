using System.Data;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FoodplannerDataAccessSql.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Test.MigrationTests;

public class RestructureFoodplannerTests
{
    // Capture FluentMigrator instructions without executing them against a database.
    private static List<IMigrationExpression> GetExpressions(
        RestructureFoodplanner migration, bool up)
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

    [Fact]
    public void Up_RemovesForeignKeysToCoreOwnedTables()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedKeys = new[]
        {
            ("meals", "fk_meals_Image_id"),
            ("meals", "fk_meals_user_id"),
            ("ingredients", "fk_ingredients_Image_id"),
            ("ingredients", "fk_ingredients_user_id"),
            ("subingredients", "fk_subingredients_Image_id"),
            ("subingredients", "fk_subingredients_user_id")
        };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var keys = expressions.OfType<DeleteForeignKeyExpression>()
            .Select(e => (e.ForeignKey.ForeignTable, e.ForeignKey.Name))
            .ToList();

        // Assert
        Assert.Equal(expectedKeys.Length, keys.Count);
        foreach (var key in expectedKeys)
            Assert.Contains(key, keys);
    }

    [Fact]
    public void Up_RemovesCoreTablesAfterTheirForeignKeys()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedTables = new[]
        {
            "one_time_password", "message", "chat_thread", "child_relation",
            "children", "classroom", "food_image", "users"
        };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var removedTables = expressions.OfType<DeleteTableExpression>()
            .Select(e => e.TableName)
            .ToList();
        var lastForeignKey = expressions.FindLastIndex(e => e is DeleteForeignKeyExpression);
        var firstCoreTable = expressions.FindIndex(e =>
            e is DeleteTableExpression t && t.TableName == "one_time_password");

        // Assert
        Assert.Equal(expectedTables, removedTables.Take(expectedTables.Length));
        Assert.True(lastForeignKey >= 0);
        Assert.True(firstCoreTable > lastForeignKey);
    }

    [Fact]
    public void Up_RenamesImageReferences()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var renames = expressions.OfType<RenameColumnExpression>().ToList();

        // Assert
        Assert.Contains(renames, r => r.TableName == "ingredients"
            && r.OldName == "Image_id" && r.NewName == "image_id");
        Assert.Contains(renames, r => r.TableName == "meals"
            && r.OldName == "Image_id" && r.NewName == "image_id");
    }

    [Fact]
    public void Up_RenamesPackedTableAndOrderingColumn()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var tableRenames = expressions.OfType<RenameTableExpression>().ToList();
        var columnRenames = expressions.OfType<RenameColumnExpression>().ToList();

        // Assert
        Assert.Contains(tableRenames, r =>
            r.OldName == "packed_ingredients" && r.NewName == "packed");
        Assert.Contains(columnRenames, r =>
            r.TableName == "packed" && r.OldName == "order_no"
            && r.NewName == "order_no");
    }

    [Fact]
    public void Up_ReplacesPackedIdWithCompositePrimaryKey()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedKeyColumns = new[] { "ingredient_id", "meal_id" };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var oldKeys = expressions.OfType<DeleteConstraintExpression>()
            .Where(e => e.Constraint.ConstraintName == "PK_packed_ingredients")
            .ToList();
        var removedColumns = expressions.OfType<DeleteColumnExpression>().ToList();
        var newKeys = expressions.OfType<CreateConstraintExpression>()
            .Where(e => e.Constraint.ConstraintName == "pk_packed")
            .ToList();

        // Assert
        var oldKey = Assert.Single(oldKeys);
        Assert.True(oldKey.Constraint.IsPrimaryKeyConstraint);
        Assert.Equal("packed", oldKey.Constraint.TableName);

        Assert.Contains(removedColumns, e =>
            e.TableName == "packed" && e.ColumnNames.Contains("id"));

        var newKey = Assert.Single(newKeys);
        Assert.True(newKey.Constraint.IsPrimaryKeyConstraint);
        Assert.Equal("packed", newKey.Constraint.TableName);
        Assert.Equal(expectedKeyColumns, newKey.Constraint.Columns.OrderBy(c => c));
    }

    [Fact]
    public void Up_CreatesNewSubingredientTable()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedColumns = new[] { "parent_id", "child_id", "order_no" };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var tables = expressions.OfType<CreateTableExpression>()
            .Where(e => e.TableName == "subingredient")
            .ToList();

        // Assert
        var table = Assert.Single(tables);
        Assert.Equal(expectedColumns, table.Columns.Select(c => c.Name));
        Assert.All(table.Columns, column =>
        {
            Assert.Equal(DbType.Int32, column.Type);
            Assert.Equal(false, column.IsNullable);
        });
    }

    [Fact]
    public void Up_CreatesSubingredientCompositeKeyAndTwoIngredientForeignKeys()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedKeyColumns = new[] { "child_id", "parent_id" };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var keys = expressions.OfType<CreateConstraintExpression>()
            .Where(e => e.Constraint.ConstraintName == "pk_subingredient")
            .ToList();
        var foreignKeys = expressions.OfType<CreateForeignKeyExpression>().ToList();

        // Assert
        var key = Assert.Single(keys);
        Assert.True(key.Constraint.IsPrimaryKeyConstraint);
        Assert.Equal("subingredient", key.Constraint.TableName);
        Assert.Equal(expectedKeyColumns, key.Constraint.Columns.OrderBy(c => c));

        Assert.Equal(2, foreignKeys.Count);
        Assert.Contains(foreignKeys, e => e.ForeignKey.Name == "fk_subingredient_parent"
            && e.ForeignKey.ForeignTable == "subingredient"
            && e.ForeignKey.PrimaryTable == "ingredients"
            && e.ForeignKey.ForeignColumns.SequenceEqual(new[] { "parent_id" })
            && e.ForeignKey.PrimaryColumns.SequenceEqual(new[] { "id" }));
        Assert.Contains(foreignKeys, e => e.ForeignKey.Name == "fk_subingredient_child"
            && e.ForeignKey.ForeignTable == "subingredient"
            && e.ForeignKey.PrimaryTable == "ingredients"
            && e.ForeignKey.ForeignColumns.SequenceEqual(new[] { "child_id" })
            && e.ForeignKey.PrimaryColumns.SequenceEqual(new[] { "id" }));
    }

    [Fact]
    public void Up_PreparesIdMappingAndSynchronizesIngredientSequence()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var sql = expressions.OfType<ExecuteSqlStatementExpression>()
            .Select(e => e.SqlStatement)
            .ToList();

        // Assert
        Assert.Equal(4, sql.Count);
        Assert.Contains("CREATE TEMP TABLE subingredient_id_map", sql[0]);
        Assert.Contains("old_id INTEGER PRIMARY KEY", sql[0]);
        Assert.Contains("new_id INTEGER NOT NULL UNIQUE", sql[0]);
        Assert.Contains("ON COMMIT DROP", sql[0]);
        Assert.Contains("pg_get_serial_sequence('ingredients', 'id')", sql[1]);
    }

    [Fact]
    public void Up_MapsOldSubingredientIdsToNewIngredientIds()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var sql = expressions.OfType<ExecuteSqlStatementExpression>()
            .Select(e => e.SqlStatement)
            .ToList();

        // Assert
        Assert.True(sql.Count >= 3);
        Assert.Contains("FROM subingredients", sql[2]);
        Assert.Contains("INSERT INTO ingredients", sql[2]);
        Assert.Contains("RETURNING id INTO new_ingredient_id", sql[2]);
        Assert.Contains("INSERT INTO subingredient_id_map", sql[2]);
        Assert.Contains("item.Image_id", sql[2]);
    }

    [Fact]
    public void Up_TransfersRelationsBeforeDeletingOldSubingredientTables()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var copyIndex = expressions.FindIndex(e =>
            e is ExecuteSqlStatementExpression sql
            && sql.SqlStatement.Contains("INSERT INTO subingredient ("));
        var relationDropIndex = expressions.FindIndex(e =>
            e is DeleteTableExpression t && t.TableName == "subingredient_relation");
        var subingredientsDropIndex = expressions.FindIndex(e =>
            e is DeleteTableExpression t && t.TableName == "subingredients");

        // Assert
        Assert.True(copyIndex >= 0 && copyIndex < relationDropIndex);
        Assert.True(relationDropIndex < subingredientsDropIndex);

        var copy = ((ExecuteSqlStatementExpression)expressions[copyIndex]).SqlStatement;
        Assert.Contains("r.ingredient_id", copy);
        Assert.Contains("m.new_id", copy);
        Assert.Contains("r.order_no", copy);
        Assert.Contains("m.old_id = r.subingredient_id", copy);
    }

    [Fact]
    public void Down_RemovesNewTableAndRecreatesLegacySubingredientTables()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: false);
        var removedTables = expressions.OfType<DeleteTableExpression>().ToList();
        var createdTables = expressions.OfType<CreateTableExpression>()
            .Select(e => e.TableName)
            .ToList();

        // Assert
        Assert.Equal("subingredient", Assert.Single(removedTables).TableName);
        Assert.Contains("subingredients", createdTables);
        Assert.Contains("subingredient_relation", createdTables);
    }

    [Fact]
    public void Down_RestoresOriginalSubingredientNameColumnLength()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        const int expectedLength = 100;

        // Act
        var expressions = GetExpressions(migration, up: false);
        var tables = expressions.OfType<CreateTableExpression>()
            .Where(e => e.TableName == "subingredients")
            .ToList();

        // Assert
        var table = Assert.Single(tables);
        var name = Assert.Single(table.Columns.Where(c => c.Name == "name"));
        Assert.Equal(expectedLength, name.Size);
    }

    [Fact]
    public void Down_RestoresOriginalRelationOrderNumberDefault()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        const int expectedDefault = 0;

        // Act
        var expressions = GetExpressions(migration, up: false);
        var tables = expressions.OfType<CreateTableExpression>()
            .Where(e => e.TableName == "subingredient_relation")
            .ToList();

        // Assert
        var table = Assert.Single(tables);
        var ordering = Assert.Single(table.Columns.Where(c => c.Name == "order_no"));
        Assert.Equal(expectedDefault, ordering.DefaultValue);
    }

    [Fact]
    public void Down_RestoresCascadeOnOriginalSubingredientForeignKeys()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedKeys = new[]
        {
            "fk_subingredient_relation_ingredient_id",
            "fk_subingredient_relation_subingredient_id"
        };

        // Act
        var expressions = GetExpressions(migration, up: false);
        var foreignKeys = expressions.OfType<CreateForeignKeyExpression>().ToList();

        // Assert
        foreach (var name in expectedKeys)
        {
            var fk = Assert.Single(foreignKeys.Where(e => e.ForeignKey.Name == name));
            Assert.Equal(Rule.Cascade, fk.ForeignKey.OnDelete);
        }
    }

    [Fact]
    public void Down_RestoresPackedNamesIdAndPrimaryKey()
    {
        // Arrange
        var migration = new RestructureFoodplanner();

        // Act
        var expressions = GetExpressions(migration, up: false);
        var addedColumns = expressions.OfType<CreateColumnExpression>().ToList();
        var deletedKeys = expressions.OfType<DeleteConstraintExpression>().ToList();
        var createdKeys = expressions.OfType<CreateConstraintExpression>().ToList();
        var columnRenames = expressions.OfType<RenameColumnExpression>().ToList();
        var tableRenames = expressions.OfType<RenameTableExpression>().ToList();

        // Assert
        Assert.Contains(addedColumns, e =>
            e.TableName == "packed" && e.Column.Name == "id" && e.Column.IsIdentity);
        Assert.Contains(deletedKeys, e =>
            e.Constraint.ConstraintName == "pk_packed" && e.Constraint.TableName == "packed");
        Assert.Contains(createdKeys, e =>
            e.Constraint.ConstraintName == "PK_packed_ingredients"
            && e.Constraint.TableName == "packed"
            && e.Constraint.Columns.SequenceEqual(new[] { "id" }));
        Assert.Contains(columnRenames, e =>
            e.TableName == "packed" && e.OldName == "order_no"
            && e.NewName == "order_no");
        Assert.Contains(tableRenames, e =>
            e.OldName == "packed" && e.NewName == "packed_ingredients");
    }

    [Fact]
    public void Down_RecreatesCoreTablesAndRestoresImageColumnNames()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedTables = new[]
        {
            "users", "classroom", "children", "child_relation",
            "food_image", "chat_thread", "message", "one_time_password"
        };

        // Act
        var expressions = GetExpressions(migration, up: false);
        var tables = expressions.OfType<CreateTableExpression>()
            .Select(e => e.TableName)
            .ToList();
        var renames = expressions.OfType<RenameColumnExpression>().ToList();

        // Assert
        foreach (var table in expectedTables)
            Assert.Contains(table, tables);

        foreach (var table in new[] { "ingredients", "meals" })
            Assert.Contains(renames, e =>
                e.TableName == table && e.OldName == "image_id"
                && e.NewName == "Image_id");
    }

    [Fact]
    public void Down_RecreatesForeignKeysToCoreTables()
    {
        // Arrange
        var migration = new RestructureFoodplanner();
        var expectedKeys = new[]
        {
            "fk_children_user_id", "fk_children_class_id",
            "fk_child_relation_user_id", "fk_child_relation_child_id",
            "fk_food_image_user_id", "fk_chat_thread_child_id",
            "fk_message_chat_thread_id", "fk_message_user_id",
            "fk_otp_generated_by", "fk_otp_used_by_user", "fk_otp_child_user",
            "fk_meals_Image_id", "fk_meals_user_id",
            "fk_ingredients_Image_id", "fk_ingredients_user_id",
            "fk_subingredients_Image_id", "fk_subingredients_user_id"
        };

        // Act
        var expressions = GetExpressions(migration, up: false);
        var keys = expressions.OfType<CreateForeignKeyExpression>()
            .Select(e => e.ForeignKey.Name)
            .ToList();

        // Assert
        foreach (var key in expectedKeys)
            Assert.Contains(key, keys);
    }
}
