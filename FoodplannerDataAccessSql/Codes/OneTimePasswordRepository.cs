using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FoodplannerModels.Account;
using FoodplannerModels.Codes;
using static System.Net.WebRequestMethods;

namespace FoodplannerDataAccessSql.Codes
{
    public class OneTimePasswordRepository : IOneTimePasswordRepository
    {
        private readonly PostgreSQLConnectionFactory _connectionFactory;

        public OneTimePasswordRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CheckIfCodeExistsAsync(string code)
        {
            const string sql = "SELECT code_id FROM one_time_password WHERE code = @Code";
            using var connection = _connectionFactory.Create();
            connection.Open();

            var result = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { Code = code });

            connection.Close();

            if(result == null)
                return false;
            else
                return true;
        }

        public async Task<bool> CheckIfCodeExpiredAsync(string code)
        {
            const string sql = "SELECT expires_on FROM one_time_password WHERE code = @Code";
            using var connection = _connectionFactory.Create();
            connection.Open();

            var result = await connection.QuerySingleOrDefaultAsync<DateTime?>(sql, new { Code = code });

            connection.Close();

            // We assume if none found it is an expired code
            if (result == null)
                return true;

            return result <= DateTime.UtcNow;
        }

        public async Task<int> DeleteAsync(string code)
        {
            const string sql = "DELETE FROM one_time_password WHERE code = @Code";

            using var connection = _connectionFactory.Create();
            connection.Open();

            var rowsAffected = await connection.ExecuteAsync(sql, new { Code = code });

            connection.Close();

            return rowsAffected;
        }

        public async Task<OneTimePasswordDTO?> GetFromCodeAsync(string code)
        {
            const string sql = "SELECT code_id, generated_by, used, used_by_user, child_user FROM one_time_password WHERE code = @Code";

            using var connection = _connectionFactory.Create();
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<OneTimePasswordDTO>(sql, new { Code = code });
            connection.Close();

            return result;
        }


        public async Task<int> InsertAsync(OneTimePassword createOTP)
        {
            var sql = "INSERT INTO one_time_password (generated_by, code, created_on, expires_on, used, used_by_user, child_user) VALUES (@GeneratedBy, @Code, @CreatedOn, @ExpiresOn, @Used, @UsedByUser, @ChildUser) RETURNING code_id";

            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    GeneratedBy = createOTP.GeneratedBy,
                    Code = createOTP.Code,
                    CreatedOn = createOTP.CreatedOn,
                    ExpiresOn = createOTP.ExpiresOn,
                    Used = createOTP.Used,
                    UsedByUser = createOTP.UsedByUser,
                    ChildUser = createOTP.ChildUser
                });
                connection.Close();
                return result;
            }
        }

        public async Task<int> UpdateAsync(OneTimePasswordDTO OTP)
        {
            var sql = @"
    UPDATE one_time_password 
    SET 
        generated_by = @GeneratedBy,
        used = @Used,
        used_by_user = @UsedByUser,
        child_user = @ChildUser
    WHERE code_id = @CodeId;
";

            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new
                {
                    CodeId = OTP.CodeId,
                    GeneratedBy = OTP.GeneratedBy,
                    Used = OTP.Used,
                    UsedByUser = OTP.UsedByUser,
                    ChildUser = OTP.ChildUser
                });
                connection.Close();
                return result;
            }
        }

    }
}
