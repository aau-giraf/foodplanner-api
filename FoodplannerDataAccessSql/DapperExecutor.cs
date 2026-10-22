using Dapper;

namespace FoodplannerDataAccessSql;

public class DapperExecutor(PostgreSQLConnectionFactory connectionFactory) : IDapperExecutor
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        return await connection.QuerySingleAsync<T>(sql, parameters);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        return await connection.ExecuteAsync(sql, parameters);
    }
}