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
    public static class EntityDbTranslation
    {
        // Children
        public const string ChildId = "child_id";
        public const string FirstName = "first_name";
        public const string LastName = "last_name";
        public const string parentId = "parent_id";
        public const string classId = "class_id";
        // User
        public const string Id = "id";
        public const string Email = "email";
        public const string Password = "password";
        public const string Role = "role";
        public const string RoleApproved = "role_approved";
        public const string PinCode = "pincode";
        public const string Archived = "archived";

        // Classroom
        public const string ClassName = "class_name";
        
    
        private static readonly Dictionary<string, string> _map =
            typeof(EntityDbTranslation)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .ToDictionary(f => f.Name, f => f.GetValue(null)!.ToString()!);

        public static string ToDb(string propertyName)
        {
            return _map.TryGetValue(propertyName, out var db) ? db : propertyName.ToLower();
        }
    }


    public GenericRepository(PostgreSQLConnectionFactory connectionFactory){
        _connectionFactory = connectionFactory;
    }

    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"SELECT * FROM {typeof(T).Name}";
            return await connection.QueryAsync<T>(sql);
        }
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using (var connection = _connectionFactory.Create()){
            connection.Open();
            var sql = $"SELECT * FROM {typeof(T).Name} WHERE {EntityDbTranslation.ToDb(entityId)} = @Id";
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
            var props = typeof(T).GetProperties().Skip(1);
            var sql = $"INSERT INTO {typeof(T).Name.ToLower()} ({string.Join(", ", props.Select(p => EntityDbTranslation.ToDb(p.Name)))}) VALUES ({string.Join(", ", props.Select(p => "@" + p.Name))}) RETURNING {EntityDbTranslation.ToDb(entityId)}";

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
            var props = typeof(T).GetProperties().Skip(1);
            var sql = $"UPDATE {typeof(T).Name.ToLower()} SET ({string.Join(", ", props.Select(p => $"{EntityDbTranslation.ToDb(p.Name)} = @{p.Name}"))}) WHERE {EntityDbTranslation.ToDb(entityId)} = @{entityId}";
            
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
            var sql = $"DELETE FROM {typeof(T).Name.ToLower()} WHERE {EntityDbTranslation.ToDb(entityId)} = @Id";
            Console.WriteLine(id);
            Console.WriteLine(typeof(T).Name);
            Console.WriteLine(sql);
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
