


using System.Reflection.Metadata.Ecma335;
using Dapper;
using foodplannerApi.Models;
using foodplannerApi.Services;
using Npgsql;

namespace foodplannerApi.Controller;

public static class UserControllers {
    public static void MapUserControllers(this IEndpointRouteBuilder builder) {
        builder.MapGet("users", async (PostgreConnectionFactory postgreConnectionFactory) => {

            using var connection = postgreConnectionFactory.Create();

            const string sql = "SELECT * FROM users";

            var users = await connection.QueryAsync<Users>(sql);

            return Results.Ok(users);
        });

        builder.MapGet("users/{id}", async (int id, PostgreConnectionFactory postgreConnectionFactory) => {
            using var connection = postgreConnectionFactory.Create();

            const string sql = """
            SELECT * 
            FROM users
            WHERE id = @Id
            """;

            var user = await connection.QuerySingleOrDefaultAsync<Users>(
                sql,
                new { Id = id });

            return user is not null ? Results.Ok(user) : Results.NotFound();
        });
    }
}