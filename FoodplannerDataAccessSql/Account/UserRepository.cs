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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var sql = "SELECT * FROM users WHERE email = @Email";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
                return result;
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "SELECT first_name, last_name, email FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }
            
        }

       
        public async Task<int> InsertAsync(User entity)
        {
            var sql = "INSERT INTO users (first_name, last_name, email, password, role, status) VALUES (@First_Name, @Last_Name, @Email, @Password, @role, @status) RETURNING id";
            
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new 
                {   
                    First_Name = entity.FirstName, 
                    Last_Name = entity.LastName, 
                    Email = entity.Email, 
                    Password = entity.Password,
                    role = entity.Role,
                    status = entity.Status
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
