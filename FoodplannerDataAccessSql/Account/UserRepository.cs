
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

        public async Task<int> DeleteAsync(int id)
        {
            var deleteChildrenSql = "DELETE FROM children WHERE parent_id = @Id";
            var deleteUserSql = "DELETE FROM users WHERE id = @Id";

            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(deleteChildrenSql, new { Id = id }, transaction);
                        var result = await connection.ExecuteAsync(deleteUserSql, new { Id = id }, transaction);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            var sql = "SELECT id, first_name, last_name, email, role, archived FROM users ORDER BY first_name";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<UserDTO>(sql);
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

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<UserDTO>(sql, new { Id = id });
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
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
                return count > 0;
            }
        }

        public async Task<int> InsertAsync(User entity)
        {
            var sql = "INSERT INTO users (first_name, last_name, email, password, role, role_approved) VALUES (@FirstName, @LastName, @Email, @Password, @role, @RoleApproved) RETURNING id";

            using (var connection = _connectionFactory.Create())
            {

                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    Password = entity.Password,
                    role = entity.Role,
                    RoleApproved = entity.RoleApproved
                });
                return result;

            }
        }


        public Task<int> UpdateAsync(User entity)
        {
            var sql = "UPDATE users SET first_name = @FirstName, last_name = @LastName, email = @Email, password = @Password, role = @Role, role_approved = @RoleApproved, archived = @Archived WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = connection.Execute(sql, new
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    Password = entity.Password,
                    Role = entity.Role,
                    RoleApproved = entity.RoleApproved,
                    Id = entity.Id,
                    Archived = entity.Archived
                });
                return Task.FromResult(result);
            }
        }

        public async Task<string> UpdatePinCodeAsync(string pinCode, int id)
        {
            var sql = "UPDATE users SET pincode = @PinCode WHERE id = @Id RETURNING pincode";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteScalarAsync<string>(sql, new { PinCode = pinCode, Id = id });
                return result;
            }
        }

        public async Task<string> GetPinCodeByIdAsync(int id)
        {
            var sql = "SELECT pincode FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteScalarAsync<string>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<bool> HasPinCodeAsync(int id)
        {
            var sql = "SELECT COUNT(1) FROM users WHERE id = @Id AND pincode IS NOT NULL";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });

                return result > 0;
            }
        }


        public async Task<bool> UpdateArchivedAsync(int id)
        {
            var selectSql = "SELECT archived FROM users WHERE id = @Id";
            var updateSql = " UPDATE users SET archived = @Archived WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();

                var currentArchived = await connection.ExecuteScalarAsync<bool>(selectSql, new { Id = id });

                var newArchived = !currentArchived;

                var result = await connection.ExecuteAsync(updateSql, new { Archived = newArchived, Id = id });
                return result > 0;
            }
        }

        public async Task<bool> UpdateRoleApprovedAsync(int id, bool roleApproved)
        {
            var sql = "UPDATE users SET role_approved = @RoleApproved WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { RoleApproved = roleApproved, Id = id });
                return result > 0;
            }
        }

        public async Task<IEnumerable<User?>> SelectAllNotArchivedAsync()
        {
            var sql = "SELECT * FROM users WHERE archived = false";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql);
                return result.ToList();
            }
        }
    }

}
