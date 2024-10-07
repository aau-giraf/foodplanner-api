using Dapper;
using FoodplannerModels.Account;

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
            throw new NotImplementedException();
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

       
        public  Task<int> InsertAsync(User entity)
        {
            var sql = "INSERT INTO users (first_name, last_name, email, password) VALUES (@First_Name, @Last_Name, @Email, @Password) RETURNING id";
            
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = connection.ExecuteAsync(sql, new 
                {   
                    First_Name = entity.First_name, 
                    Last_Name = entity.Last_name, 
                    Email = entity.Email, 
                    Password = entity.Password 
                });
                return result;
            }     
        }

        public Task<int> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
