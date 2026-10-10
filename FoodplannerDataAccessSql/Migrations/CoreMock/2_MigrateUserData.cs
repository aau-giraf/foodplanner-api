using FluentMigrator;
using Npgsql;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

// Holds the connection string for the source database
public class DataMigrationSource
{
    public string ConnectionString { get; }

    public DataMigrationSource(string connectionString)
    {
        ConnectionString = connectionString;
    }
}


[Migration(2)]
[Tags("CoreMock")]
public class MigrateUserData : Migration
{
    private readonly DataMigrationSource _source;

    public MigrateUserData(DataMigrationSource source)
    {
        _source = source;
    }

    public override void Up()
    {
        Execute.WithConnection((target, transaction) =>
        {
            // Connect to the existing Foodplanner database
            using var source = new NpgsqlConnection(
                _source.ConnectionString);

            source.Open();

            // Copy tables in foreign key dependency order
            CopyTable(
                source, target, transaction,
                "users",
                "id", "first_name", "last_name", "email","password", "role", "pincode","role_approved", "archived");

            CopyTable(
                source, target, transaction,
                "classroom",
                "class_id", "class_name");

            CopyTable(
                source, target, transaction,
                "children",
                "child_id", "first_name", "last_name","class_id");

            CopyTable(
                source, target, transaction,
                "child_relation",
                "user_id", "child_id");

            // Update identity sequences after inserting old IDs
            UpdateSequence(target, transaction, "users", "id");
            UpdateSequence(target, transaction, "classroom", "class_id");

            Console.WriteLine(
                "User data migration completed successfully.");
        });
    }

    private static void CopyTable(
        NpgsqlConnection source,
        IDbConnection target,
        IDbTransaction transaction,
        string table,
        params string[] columns)
    {
        string columnList = string.Join(", ", columns);
        string parameters = string.Join(", ",
            columns.Select(c => "@" + c));

        using var select = new NpgsqlCommand(
            $"SELECT {columnList} FROM {table}", source);

        using var reader = select.ExecuteReader();

        using var insert = target.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText =$"INSERT INTO {table} ({columnList}) " + $"OVERRIDING SYSTEM VALUE VALUES ({parameters})";

        // Create parameters once, then reuse them for every row
        foreach (var column in columns)
        {
            var parameter = insert.CreateParameter();
            parameter.ParameterName = column;
            insert.Parameters.Add(parameter);
        }

        int copied = 0;

        while (reader.Read())
        {
            for (int i = 0; i < columns.Length; i++)
            {
                var parameter =
                    (IDbDataParameter)insert.Parameters[i]!;

                parameter.Value = reader.GetValue(i);
            }

            insert.ExecuteNonQuery();
            copied++;
        }

        Console.WriteLine($"Copied {copied} rows from {table}");
    }

    private static void UpdateSequence(
        IDbConnection target,
        IDbTransaction transaction,
        string table,
        string idColumn)
    {
        using var command = target.CreateCommand();
        command.Transaction = transaction;

        command.CommandText = $"""
            SELECT setval(
                pg_get_serial_sequence('{table}', '{idColumn}'),
                GREATEST(COALESCE(MAX({idColumn}), 0), 1),
                MAX({idColumn}) IS NOT NULL
            )
            FROM {table};
            """;

        command.ExecuteNonQuery();
    }

    public override void Down()
    {
        throw new NotSupportedException(
            "User data migration cannot be automatically rolled back.");
    }
}