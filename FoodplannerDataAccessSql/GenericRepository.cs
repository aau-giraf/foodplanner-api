using System.Security.Cryptography.X509Certificates;
using Dapper;
using FoodplannerModels;
using Microsoft.AspNetCore.Authentication;

namespace FoodplannerDataAccessSql;


// UserRepositoryImpl.cs
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    protected virtual string entityId => "Id";

    public GenericRepository(PostgreSQLConnectionFactory connectionFactory){
        _connectionFactory = connectionFactory;
    }

    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"SELECT * FROM {EntityDbTranslation.ToDb(typeof(T).Name)}";
            return await connection.QueryAsync<T>(sql);
        }
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();

            var tableName = EntityDbTranslation.ToDb(typeof(T).Name);
            var idColumn  = EntityDbTranslation.ToDb(entityId);

            var sql = $"""
                    SELECT *
                    FROM {tableName}
                    WHERE {idColumn} = @Id
                    """;
Console.WriteLine("Using General Repository: 'GetByIdAsync' using:\n");
Console.WriteLine("Entity:\n" + id);
Console.WriteLine(sql);
            return await connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }
    }

    public async Task<int> InsertAsync(T entity)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var props = typeof(T).GetProperties().Where(p => p.Name != entityId);
            var tableName = EntityDbTranslation.ToDb(typeof(T).Name);
            var columns = string.Join(", ", props.Select(p => EntityDbTranslation.ToDb(p.Name)));
            var parameters = string.Join(", ", props.Select(p => "@" + p.Name));
            var idColumn = EntityDbTranslation.ToDb(entityId);

            var sql = $"""
                INSERT INTO {tableName}
                ({columns})
                VALUES ({parameters})
                RETURNING {idColumn}
                """;

// temp
var propsss = typeof(T).GetProperties()
                     .Select(p => $"{p.Name} = {p.GetValue(entity)}");
Console.WriteLine("Using General Repository: 'InsertAsync' using:\n");
Console.WriteLine("Entity:\n" + string.Join("\n", propsss));
Console.WriteLine(sql);
Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");

            return await connection.ExecuteAsync(sql, entity);
        }
    }

    public async Task<int> UpdateAsync(T entity)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var props = typeof(T).GetProperties().Where(p => p.Name != entityId);
            
            var tableName = EntityDbTranslation.ToDb(typeof(T).Name);
            var updateParameters = string.Join(", ", props.Select(p => $"{EntityDbTranslation.ToDb(p.Name)} = @{p.Name}"));
            var idColumn = EntityDbTranslation.ToDb(entityId);
            var sql = $"""
                    UPDATE {tableName} 
                    SET ({updateParameters}) 
                    WHERE {EntityDbTranslation.ToDb(entityId)} = @{entityId}
                    """;
            
// temp
var propsss = typeof(T).GetProperties()
                     .Select(p => $"{p.Name} = {p.GetValue(entity)}");
Console.WriteLine("Using General Repository: 'UpdateAsync' using:\n");
Console.WriteLine("Entity:\n" + string.Join("\n", propsss));
Console.WriteLine(sql);
Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            var Result = await connection.ExecuteAsync(sql, entity);
            Console.WriteLine(Result);
            Console.WriteLine("\n\n\n\n\n\n\n\n\n");
            return Result;
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var tableName = EntityDbTranslation.ToDb(typeof(T).Name);
            var idColumn = EntityDbTranslation.ToDb(entityId);

            var sql = $"""
                    DELETE FROM {tableName} 
                    WHERE {idColumn} = @{entityId}
                    """;
            Console.WriteLine(id);
            Console.WriteLine(typeof(T).Name);
            Console.WriteLine(sql);
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
