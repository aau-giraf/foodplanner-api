
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
            var sql = "SELECT * FROM classroom ORDER BY class_name";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<Classroom>(sql);
                return result.ToList();
            } 
        }
        public async Task<int> InsertAsync(CreateClassroomDTO entity)
        {
            var sql = "INSERT INTO classroom (class_name) VALUES (@ClassName) RETURNING class_id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new{
                    ClassName = entity.ClassName
                });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CreateClassroomDTO entity, int id)
        {
            var sql = "UPDATE classroom SET class_name = @ClassName WHERE class_id = @ClassRoomId RETURNING class_id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new{
                    entity.ClassName,
                    ClassRoomId = id
                });
                return result;
            }
        }

        public async Task<bool> CheckChildrenInClassroom(int id)
        {
            var sql = "SELECT COUNT(*) FROM children WHERE class_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return result > 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM classroom WHERE class_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }
    }
}
