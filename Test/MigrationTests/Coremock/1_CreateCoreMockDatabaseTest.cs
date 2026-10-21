using System.Data;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FoodplannerDataAccessSql.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Test.MigrationTests;

public class CreateCoreMockDatabaseTests
{
    // Collect FluentMigrator expressions without connecting to a database.
    private static List<IMigrationExpression> GetExpressions(CreateCoreMockDatabase migration, bool up)
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
    public void Up_CreatesAllEightCoreTables()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();
        var expectedTables = new[]
        {
            "users", "classroom", "children", "child_relation",
            "food_image", "message", "chat_thread", "one_time_password"
        };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var tables = expressions.OfType<CreateTableExpression>()
            .Select(t => t.TableName)
            .ToList();

        // Assert
        Assert.Equal(expectedTables, tables);
    }

    [Fact]
    public void Up_CreatesUsersWithCorrectColumns()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();
        var expectedColumns = new[]
        {
            "id", "first_name", "last_name", "email", "password",
            "role", "pincode", "role_approved", "archived"
        };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var userTables = expressions.OfType<CreateTableExpression>()
            .Where(t => t.TableName == "users");

        // Assert
        var table = Assert.Single(userTables);
        Assert.Equal(expectedColumns, table.Columns.Select(c => c.Name));

        var id = Assert.Single(table.Columns.Where(c => c.Name == "id"));
        Assert.Equal(DbType.Int32, id.Type);
        Assert.True(id.IsPrimaryKey);
        Assert.True(id.IsIdentity);

        Assert.Equal(30, Assert.Single(table.Columns.Where(c => c.Name == "first_name")).Size);
        Assert.Equal(100, Assert.Single(table.Columns.Where(c => c.Name == "last_name")).Size);
        Assert.Equal(100, Assert.Single(table.Columns.Where(c => c.Name == "email")).Size);
        Assert.Equal(100, Assert.Single(table.Columns.Where(c => c.Name == "password")).Size);
        Assert.Equal(30, Assert.Single(table.Columns.Where(c => c.Name == "role")).Size);

        var pincode = Assert.Single(table.Columns.Where(c => c.Name == "pincode"));
        Assert.True(pincode.IsNullable);
        Assert.Equal(100, pincode.Size);

        foreach (var columnName in new[] { "role_approved", "archived" })
        {
            var column = Assert.Single(table.Columns.Where(c => c.Name == columnName));
            Assert.Equal(DbType.Boolean, column.Type);
            Assert.False(column.IsNullable);
        }
    }

    [Fact]
    public void Up_ConfiguresClassroomAndChildrenKeys()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var tables = expressions.OfType<CreateTableExpression>().ToList();

        // Assert
        var classroom = Assert.Single(tables.Where(t => t.TableName == "classroom"));
        var classroomId = Assert.Single(classroom.Columns.Where(c => c.Name == "class_id"));
        Assert.True(classroomId.IsPrimaryKey);
        Assert.True(classroomId.IsIdentity);
        Assert.Contains(classroom.Columns, c => c.Name == "class_name" && c.IsNullable == false);

        var children = Assert.Single(tables.Where(t => t.TableName == "children"));
        var childId = Assert.Single(children.Columns.Where(c => c.Name == "child_id"));
        Assert.True(childId.IsPrimaryKey);
        Assert.False(childId.IsIdentity); // Children reuse the corresponding user's ID.
        Assert.Contains(children.Columns, c => c.Name == "class_id" && c.IsNullable == true);
    }

    [Fact]
    public void Up_CreatesChildRelationWithCompositePrimaryKey()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();

        // Act
        var expressions = GetExpressions(migration, up: true);

        // Assert
        var table = Assert.Single(expressions.OfType<CreateTableExpression>()
            .Where(t => t.TableName == "child_relation"));

        Assert.Equal(new[] { "user_id", "child_id" }, table.Columns.Select(c => c.Name));
        Assert.All(table.Columns, c =>
        {
            Assert.Equal(DbType.Int32, c.Type);
            Assert.False(c.IsNullable);
        });

        var key = Assert.Single(expressions.OfType<CreateConstraintExpression>()
            .Where(e => e.Constraint.ConstraintName == "PK_child_relation"));
        Assert.True(key.Constraint.IsPrimaryKeyConstraint);
        Assert.Equal("child_relation", key.Constraint.TableName);
        Assert.Equal(new[] { "child_id", "user_id" },
            key.Constraint.Columns.OrderBy(c => c));
    }

    [Fact]
    public void Up_CreatesFoodImageWithCorrectColumns()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var table = Assert.Single(
            expressions.OfType<CreateTableExpression>()
                .Where(t => t.TableName == "food_image"));

        // Assert
        Assert.Contains(table.Columns, c =>
            c.Name == "id" && c.IsPrimaryKey && c.IsIdentity);

        Assert.Contains(table.Columns, c =>
            c.Name == "image_id" &&
            c.Type == DbType.String &&
            c.IsNullable == false);

        Assert.Contains(table.Columns, c =>
            c.Name == "user_id" &&
            c.Type == DbType.Int32 &&
            c.IsNullable == false);

        Assert.Contains(table.Columns, c =>
            c.Name == "image_name" &&
            c.Type == DbType.String &&
            c.IsNullable == false);

        Assert.Contains(table.Columns, c =>
            c.Name == "image_file_type" &&
            c.Type == DbType.String &&
            c.IsNullable == false);

        Assert.Contains(table.Columns, c =>
            c.Name == "size" &&
            c.Type == DbType.Int64 &&
            c.IsNullable == false);
    }

    [Fact]
    public void Up_CreatesChatTablesWithMessageDefaults()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var tables = expressions.OfType<CreateTableExpression>().ToList();

        // Assert
        var chatThread = Assert.Single(tables.Where(t => t.TableName == "chat_thread"));
        var threadId = Assert.Single(chatThread.Columns.Where(c => c.Name == "chat_thread_id"));
        Assert.True(threadId.IsPrimaryKey);
        Assert.True(threadId.IsIdentity);
        Assert.Contains(chatThread.Columns, c => c.Name == "child_id" && c.IsNullable == false);

        var message = Assert.Single(tables.Where(t => t.TableName == "message"));
        Assert.Equal(1000, Assert.Single(message.Columns.Where(c => c.Name == "content")).Size);

        foreach (var name in new[] { "archived", "is_edited" })
        {
            var column = Assert.Single(message.Columns.Where(c => c.Name == name));
            Assert.Equal(DbType.Boolean, column.Type);
            Assert.False(column.IsNullable);
            Assert.Equal(false, column.DefaultValue);
        }
    }

    [Fact]
    public void Up_CreatesOneTimePasswordWithCorrectDefaultsAndNullability()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();

        // Act
        var expressions = GetExpressions(migration, up: true);

        // Assert
        var table = Assert.Single(expressions.OfType<CreateTableExpression>()
            .Where(t => t.TableName == "one_time_password"));

        var code = Assert.Single(table.Columns.Where(c => c.Name == "code"));
        Assert.Equal(6, code.Size);
        Assert.True(code.IsUnique);
        Assert.False(code.IsNullable);

        var created = Assert.Single(table.Columns.Where(c => c.Name == "created_on"));
        Assert.Equal(SystemMethods.CurrentDateTime, created.DefaultValue);

        var used = Assert.Single(table.Columns.Where(c => c.Name == "used"));
        Assert.Equal(false, used.DefaultValue);
        Assert.False(used.IsNullable);

        Assert.True(Assert.Single(table.Columns.Where(c => c.Name == "used_by_user")).IsNullable);
        Assert.True(Assert.Single(table.Columns.Where(c => c.Name == "child_user")).IsNullable);
    }

    [Fact]
    public void Up_CreatesAllElevenForeignKeysWithCorrectReferences()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();
        var expected = new[]
        {
            ("FK_children_users", "children", "child_id", "users", "id"),
            ("FK_children_classroom", "children", "class_id", "classroom", "class_id"),
            ("FK_child_relation_users", "child_relation", "user_id", "users", "id"),
            ("FK_child_relation_children", "child_relation", "child_id", "children", "child_id"),
            ("FK_message_chat_thread", "message", "chat_thread_id", "chat_thread", "chat_thread_id"),
            ("FK_message_users", "message", "user_id", "users", "id"),
            ("FK_chat_thread_children", "chat_thread", "child_id", "children", "child_id"),
            ("FK_food_image_users", "food_image", "user_id", "users", "id"),
            ("FK_otp_generated_by", "one_time_password", "generated_by", "users", "id"),
            ("FK_otp_used_by_user", "one_time_password", "used_by_user", "users", "id"),
            ("FK_otp_child_user", "one_time_password", "child_user", "children", "child_id")
        };

        // Act
        var expressions = GetExpressions(migration, up: true);
        var keys = expressions.OfType<CreateForeignKeyExpression>()
            .Select(e => e.ForeignKey)
            .ToList();

        // Assert
        Assert.Equal(expected.Length, keys.Count);

        foreach (var (name, fromTable, fromColumn, toTable, toColumn) in expected)
        {
            var key = Assert.Single(keys.Where(k => k.Name == name));
            Assert.Equal(fromTable, key.ForeignTable);
            Assert.Equal(new[] { fromColumn }, key.ForeignColumns);
            Assert.Equal(toTable, key.PrimaryTable);
            Assert.Equal(new[] { toColumn }, key.PrimaryColumns);
        }
    }

    [Fact]
    public void Up_CreatesForeignKeysAfterAllTables()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();

        // Act
        var expressions = GetExpressions(migration, up: true);
        var lastTable = expressions.FindLastIndex(e => e is CreateTableExpression);
        var firstForeignKey = expressions.FindIndex(e => e is CreateForeignKeyExpression);

        // Assert
        Assert.True(lastTable >= 0);
        Assert.True(firstForeignKey > lastTable);
    }

    [Fact]
    public void Down_RemovesAllCoreTablesInDependencyOrder()
    {
        // Arrange
        var migration = new CreateCoreMockDatabase();
        var expectedTables = new[]
        {
            "one_time_password", "message", "chat_thread", "child_relation",
            "food_image", "children", "classroom", "users"
        };

        // Act
        var expressions = GetExpressions(migration, up: false);
        var deleted = expressions.OfType<DeleteTableExpression>()
            .Select(t => t.TableName)
            .ToList();

        // Assert
        Assert.Equal(8, expressions.Count);
        Assert.Equal(expectedTables, deleted);
    }
}