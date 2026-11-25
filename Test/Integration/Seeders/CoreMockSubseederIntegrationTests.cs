using DummyData.CoreMock;
using Npgsql;

namespace Test.Integration.Seeder;

[Collection("IntegrationDatabase")]
public class CoreMockSubseederIntegrationTests
{
    private readonly DatabaseFixture fixture;

    public CoreMockSubseederIntegrationTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CoreMockSeeder_ShouldSeedAllFiveTablesAndTheirRelationships()
    {
        // Arrange
        var createdFiles = new List<string>();
        try
        {
            await PrepareImagesAsync(createdFiles);

            await using var connection = fixture.ConnectionFactory.Create();
            Assert.Equal("giraf_test", new NpgsqlConnectionStringBuilder(connection.ConnectionString).Database);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await CreateCoreMockTablesAsync(connection, transaction);

            // Act
            await CoreMockSeeder.SeedAsync(connection, transaction);

            // Assert
            Assert.Equal(10, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM users"));
            Assert.Equal(2, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM classroom"));
            Assert.Equal(4, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM children"));
            Assert.Equal(8, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM child_relation"));
            Assert.Equal(20, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM food_image"));

            Assert.Equal(1, await CountAsync(connection, transaction,
                "SELECT COUNT(*) FROM users WHERE email = 'admin@example.com' AND role = 'Admin'"));
            Assert.Equal(1, await CountAsync(connection, transaction,
                "SELECT COUNT(*) FROM classroom WHERE class_name = 'PE'"));

            Assert.Equal(1, await CountAsync(connection, transaction, """
                SELECT COUNT(*) FROM children c
                JOIN users u ON u.id = c.child_id
                JOIN classroom cl ON cl.class_id = c.class_id
                WHERE u.email = 'child@example.com' AND cl.class_name = 'PE'
                """));
            Assert.Equal(1, await CountAsync(connection, transaction, """
                SELECT COUNT(*) FROM child_relation cr
                JOIN users parent ON parent.id = cr.user_id
                JOIN users child ON child.id = cr.child_id
                WHERE parent.email = 'parent@example.com' AND child.email = 'child@example.com'
                """));
            Assert.Equal(1, await CountAsync(connection, transaction, """
                SELECT COUNT(*) FROM child_relation cr
                JOIN users teacher ON teacher.id = cr.user_id
                JOIN users child ON child.id = cr.child_id
                WHERE teacher.email = 'teacher@example.com' AND child.email = 'child@example.com'
                """));
            Assert.Equal(1, await CountAsync(connection, transaction, """
                SELECT COUNT(*) FROM food_image fi
                JOIN users u ON u.id = fi.user_id
                WHERE u.email = 'parent@example.com'
                  AND fi.image_name = 'Test''s Madpakke'
                  AND fi.image_file_type = 'PNG'
                  AND fi.size > 0
                  AND octet_length(fi.image_data) = fi.size
                """));
            Assert.Equal(20, await CountAsync(connection, transaction, """
                SELECT COUNT(*) FROM food_image
                WHERE image_file_type = 'PNG'
                  AND size > 0
                  AND octet_length(image_data) = size
                """));

            await transaction.RollbackAsync();
        }
        finally
        {
            foreach (var file in createdFiles)
                File.Delete(file);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CoreMockSeeder_ShouldNotInsertDuplicatesWhenCalledTwice()
    {
        // Arrange
        var createdFiles = new List<string>();
        try
        {
            await PrepareImagesAsync(createdFiles);

            await using var connection = fixture.ConnectionFactory.Create();
            Assert.Equal("giraf_test", new NpgsqlConnectionStringBuilder(connection.ConnectionString).Database);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await CreateCoreMockTablesAsync(connection, transaction);

            // Act
            await CoreMockSeeder.SeedAsync(connection, transaction);
            await CoreMockSeeder.SeedAsync(connection, transaction);

            // Assert
            Assert.Equal(10, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM users"));
            Assert.Equal(2, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM classroom"));
            Assert.Equal(4, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM children"));
            Assert.Equal(8, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM child_relation"));
            Assert.Equal(20, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM food_image"));

            await transaction.RollbackAsync();
        }
        finally
        {
            foreach (var file in createdFiles)
                File.Delete(file);
        }
    }

    private static async Task CreateCoreMockTablesAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        await using var command = new NpgsqlCommand("""
            CREATE TEMP TABLE users (
                id integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                first_name varchar(30) NOT NULL,
                last_name varchar(100) NOT NULL,
                email varchar(100) NOT NULL UNIQUE,
                password varchar(100) NOT NULL,
                role varchar(30) NOT NULL,
                pincode varchar(100),
                role_approved boolean NOT NULL
            );

            CREATE TEMP TABLE classroom (
                class_id integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                class_name text NOT NULL
            );

            CREATE TEMP TABLE children (
                child_id integer PRIMARY KEY REFERENCES users(id),
                first_name varchar(100) NOT NULL,
                last_name varchar(100) NOT NULL,
                class_id integer REFERENCES classroom(class_id)
            );

            CREATE TEMP TABLE child_relation (
                user_id integer NOT NULL REFERENCES users(id),
                child_id integer NOT NULL REFERENCES children(child_id),
                PRIMARY KEY (user_id, child_id)
            );

            CREATE TEMP TABLE food_image (
                id integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                user_id integer NOT NULL REFERENCES users(id),
                image_name text NOT NULL,
                image_file_type text NOT NULL,
                size bigint NOT NULL,
                image_data bytea NOT NULL
            );
            """, connection, transaction);

        await command.ExecuteNonQueryAsync();
    }

    private static async Task PrepareImagesAsync(List<string> createdFiles)
    {
        var images = new (string Folder, string[] Names)[]
        {
            ("Lunchboxes", new[] { "lunchbox1.png", "lunchbox2.png", "lunchbox3.png", "lunchbox4.png" }),
            ("Ingredients", new[]
            {
                "Ingrediens1.png", "Ingrediens2.png", "Ingrediens3.png", "Ingrediens5.png",
                "Subingrediens1.png", "Subingrediens2.png", "Subingrediens3.png",
                "Subingrediens4.png", "Subingrediens5.png", "Subingrediens6.png",
                "Subingrediens7.png", "Subingrediens8.png", "Subingrediens9.png",
                "Subingrediens10.png", "Subingrediens11.png"
            })
        };

        byte[] png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/XWcAAAAASUVORK5CYII=");

        foreach (var (folder, names) in images)
        {
            foreach (var name in names)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "CoreMock", "Images", folder, name);
                if (File.Exists(path))
                    continue;

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                await File.WriteAllBytesAsync(path, png);
                createdFiles.Add(path);
            }
        }
    }

    private static async Task<long> CountAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        return (long)(await command.ExecuteScalarAsync())!;
    }
}
