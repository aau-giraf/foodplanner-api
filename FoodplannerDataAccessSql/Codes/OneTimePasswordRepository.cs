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

        public async Task<OneTimePassword?> GetFromCodeAsync(string code)
        {
            const string sql = "SELECT * FROM one_time_password WHERE code = @Code";

            using var connection = _connectionFactory.Create();
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<OneTimePassword>(sql, new { Code = code });
            Console.WriteLine(result.CreatedOn);
            return result;
        }


        public async Task<int> InsertAsync(OneTimePassword createOTP)
        {
            var sql = "INSERT INTO one_time_password (generated_by, code, created_on, expires_on, used, used_by_user) VALUES (@GeneratedBy, @Code, @CreatedOn, @ExpiresOn, @Used, @UsedByUser) RETURNING code_id";

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
                });
                return result;
            }
        }

        public async Task<int> UpdateAsync(OneTimePassword OTP)
        {
            var sql = @"
        UPDATE one_time_password 
        SET 
            generated_by = @GeneratedBy,
            code = @Code,
            created_on = @CreatedOn,
            expires_on = @ExpiresOn,
            used = @Used,
            used_by_user = @UsedByUser
        WHERE code_id = @CodeId;
    ";

            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new
                {
                    CodeId = OTP.CodeId,
                    GeneratedBy = OTP.GeneratedBy,
                    Code = OTP.Code,
                    CreatedOn = OTP.CreatedOn,
                    ExpiresOn = OTP.ExpiresOn,
                    Used = OTP.Used,
                    UsedByUser = OTP.UsedByUser
                });
                return result;
            }
        }

    }
}
