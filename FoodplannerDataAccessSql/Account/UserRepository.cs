
using Dapper;
using FoodplannerModels.Account;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{
    public class UserRepository : IUserRepository
    {

        private readonly PostgreSQLConnectionFactory _connectionFactory;

        public UserRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = connection.Execute(sql, new { Id = id });
                return Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT first_name, last_name, email FROM users";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql);
                return result.ToList();
            }
        }

        public async Task<User?> GetByEmailAndPasswordAsync(string email, string password)
        {
            var sql = "SELECT * FROM users WHERE email = @Email AND password = @Password";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email, Password = password });
                return result;
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var sql = "SELECT first_name, last_name, email FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }
            
        }

        public async Task<IEnumerable<User>> GetAllNotApprovedAsync()
        {
            var sql = "SELECT id, first_name, last_name, email, role FROM users WHERE role_approved = false";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql);
                return result.ToList();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var sql = "SELECT COUNT(1) FROM users WHERE email = @Email";
            using (var connection = _connectionFactory.Create()){
                connection.Open();
                var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
                return count > 0;
            }
        }

        public async Task<int> InsertAsync(User entity)
        {
            var sql = "INSERT INTO users (first_name, last_name, email, password, role, role_approved) VALUES (@First_Name, @Last_Name, @Email, @Password, @Role, @Role_approved) RETURNING id";
            
            using (var connection = _connectionFactory.Create())
            {
            
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new 
                {   
                    First_Name = entity.First_name, 
                    Last_Name = entity.Last_name, 
                    Email = entity.Email, 
                    Password = entity.Password, 
                    Role = entity.Role,
                    Role_approved = false
                });
                return result;
            
            }     
        }

        public async Task<int> ApproveRoleAsync(int id)
        {
            var sql = "UPDATE users SET role_approved = true WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public Task<int> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
