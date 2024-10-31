
using Dapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{
    public class ClassroomRepository : IClassroomRepository
    {

        private readonly PostgreSQLConnectionFactory _connectionFactory;

        public ClassroomRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<Classroom>> GetAllAsync()
        {
            var sql = "SELECT * FROM classroom";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<Classroom>(sql);
                return result.ToList();
            } 
        }
    }
}
