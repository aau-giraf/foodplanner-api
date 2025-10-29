using Dapper;
using FoodplannerModels;

namespace FoodplannerDataAccessSql;


// UserRepositoryImpl.cs
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    public GenericRepository(PostgreSQLConnectionFactory connectionFactory){
        _connectionFactory = connectionFactory;
    }

    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"SELECT * FROM {typeof(T).Name.ToLower()}";
            return await connection.QueryAsync<T>(sql);
        }
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"SELECT * FROM {typeof(T).Name.ToLower()} WHERE Id = @Id";
            var result = await connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
            if (result == null)
            {
                return null;
            }
            return result;
        }
    }

    public async Task<int> InsertAsync(T entity)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"INSERT INTO {typeof(T).Name.ToLower()} {string.Join(", ", GetProperties(entity))} VALUES ({string.Join(", ", GetProperties(entity, "@"))})";
            return await connection.ExecuteAsync(sql, entity);
        }
    }

    public async Task<int> UpdateAsync(T entity)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"SELECT * FROM {typeof(T).Name.ToLower()} SET {string.Join(", ", GetUpdateFields(entity))} WHERE Id = @Id";
            return await connection.ExecuteAsync(sql, entity);
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"DELETE FROM {typeof(T).Name.ToLower()} WHERE Id = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

    private IEnumerable<string> GetProperties(T entity, string prefix = "")
    {
        return typeof(T).GetProperties().Select(p => $"{prefix}{p.Name}");
    }

    private IEnumerable<string> GetUpdateFields(T entity)
    {
        return typeof(T).GetProperties().Where(p => p.Name != "Id").Select(p => $"{p.Name} = @{p.Name}");
    }

}
