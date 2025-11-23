using Dapper;
using FoodplannerModels.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerDataAccessSql.Codes
{
    public class OneTimePasswordRepository : IOneTimePasswordRepository
    {
        private readonly PostgreSQLConnectionFactory _connectionFactory;

        public OneTimePasswordRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
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
    }
}
