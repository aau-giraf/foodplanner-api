using Dapper;
using foodplanner_models.Account;

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

        public Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
