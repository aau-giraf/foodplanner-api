
using Dapper;
using FoodplannerModels;
using FoodplannerModels.Account;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{
    public class UserRepository : IUserRepository
    {

        private readonly PostgreSQLConnectionFactory _connectionFactory;

        // Constructor for user repository
        public UserRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /* Asynchronously deletes a user from the users table based on their ID
        Uses a transaction to roll back the deletion if an error occurs*/
        public async Task<int> DeleteAsync(int id)
        {
            var deleteUserSql = "DELETE FROM users WHERE id = @Id";

            using var connection = _connectionFactory.Create();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
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

        //Asynchronously retrieves all users from the users table, ordered by first name
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT id, first_name, last_name, email, role, archived FROM users ORDER BY first_name";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql);
                return result.ToList();
            }
        }

        //Asynchronously retrieves a user from the users table based on their email address
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

        // Asynchronously retrieves a user from the users table based on their ID
        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "SELECT id, first_name, last_name, email, role, role_approved FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }

        }

        // Asynchronously retrieves all users whose roles have not been approved
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

        // Asynchronously checks if a user with the given email exists returns true if the user exists, false otherwise
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

        // Asynchronously inserts a new user into the users table and returns the ID of the newly inserted user
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
                    role = entity.Role.ToString(),
                    RoleApproved = entity.RoleApproved
                });
                return result;

            }
        }

        // updates an existing user's first name, last name, email, and password based on the user's ID
        public Task<int> UpdateAsync(User entity)
        {
            var sql = "UPDATE users SET first_name = @FirstName, last_name = @LastName, email = @Email, password = @Password WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = connection.Execute(sql, new
                {
                    Id = entity.Id,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    Password = entity.Password,
                    Role = entity.Role.ToString(),
                    RoleApproved = entity.RoleApproved,
                    Archived = entity.Archived
                });
                return Task.FromResult(result);
            }
        }

        // Asynchronously updates the pincode of a user based on their ID and returns the updated pincode
        public async Task<string> UpdatePinCodeAsync(string pinCode, int id)
        {
            var sql = "UPDATE users SET pincode = @PinCode WHERE id = @Id RETURNING pincode";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteScalarAsync<string>(sql, new { PinCode = pinCode, Id = id });

                connection.Close();
                
                if (result != null)
                {
                    return result;
                }
                else 
                {
                    throw new Exception("Failed to update pincode");
                }

            }
        }

        // Asynchronously retrieves a user's pincode based on their ID, throws an exception if the pincode is not found
        public async Task<string> GetPinCodeByIdAsync(int id)
        {
            var sql = "SELECT pincode FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteScalarAsync<string>(sql, new { Id = id });
                connection.Close();
                
                if(result != null)
                {
                    return result;
                }
                else
                {
                    throw new Exception("Pincode not found");
                }
            }
        }

        // Asynchronously checks whether a user has a pincode, returns true if the user has a pincode, false otherwise
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

        // Asynchronously toggles a user's archived status, returns true if the archived status was sucessfully updated
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

        // Asynchronously updates whether a user's role has been approved
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

        // Asynchronously retrieves all users who are not archived.
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

        // Asynchronously retrieves the logged-in user's information based on their ID, throws an exception if the user is not found
        public async Task<User> GetLoggedInAsync(int id)
        {
            var sql = "SELECT id, first_name, last_name, email, role, role_approved FROM users WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
                connection.Close();
                
                if(result != null)
                {
                    return result;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
        }

        // Asynchronously updates the logged-in user's first name, last name, and email based on their ID
        public async Task<int> UpdateLoggedInAsync(int id, UserUpdateLoggedInDTO userUpdateLoggedInDto)
        {
            var sql = "UPDATE users SET first_name = @FirstName, last_name = @LastName, email = @Email WHERE id = @Id RETURNING id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    FirstName = userUpdateLoggedInDto.FirstName,
                    LastName = userUpdateLoggedInDto.LastName,
                    Email = userUpdateLoggedInDto.Email,
                });
                return result;
            }
        }

        // Asynchronously updates a user's password based on their ID.
        public async Task<int> UpdatePasswordAsync(string password, int id)
        {
            var sql = "UPDATE users SET password = @Password WHERE id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Password = password, Id = id });
                return result;
            }
        }


    }
}