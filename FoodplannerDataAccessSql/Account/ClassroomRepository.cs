
using Dapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{

    // Handles operations related to the classroom table in database 
    public class ClassroomRepository : IClassroomRepository
    {

        
        private readonly PostgreSQLConnectionFactory _connectionFactory;

        // Constructor 
        public ClassroomRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Retrieve all classrooms in database, from A-Z 
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

        // Retrieve a classroom entry in the database based on the class id
        public async Task<Classroom?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM classroom WHERE class_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<Classroom>(sql, new { Id = id });
                return result;
            }
        }

        // Adds a new classroom in the database 
        public async Task<int> InsertAsync(Classroom entity)
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
        

        // Updates a classroom entry in the database 
        public async Task<int> UpdateAsync(Classroom entity)
        {
            var sql = "UPDATE classroom SET class_name = @ClassName WHERE class_id = @ClassRoomId RETURNING class_id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new{
                    entity.ClassName,
                    ClassRoomId = entity.ClassId
                });
                return result;
            }
        }

        // Returns a bool if child is found in a classroom, by counting a child's class id
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

        // Delete a class entry in classroom table given its id 
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
