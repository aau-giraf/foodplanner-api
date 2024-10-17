
using Dapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{
    public class ChildrenRepository : IChildrenRepository
    {

        private readonly PostgreSQLConnectionFactory _connectionFactory;

        public ChildrenRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Children>> GetAllAsync()
        {
            var sql = "SELECT c.*, us.first_name, us.last_name, cl.* FROM children c INNER JOIN users us ON c.id = us.id INNER JOIN classroom cl ON c.class_id = cl.class_id";
            using (var connection = _connectionFactory.Create()){
                var children = await connection.QueryAsync<Children, User, Classroom, Children>(sql, (child, user, classroom) =>
                {
                    child.user = user;
                    child.Classroom = classroom;
                    return child;
                }, splitOn: "id, class_id");
                return children;

            }
        
        }

        public async Task<int> InsertAsync(Children entity)
        {
            var sql = "INSERT INTO children (first_name, last_name, id, class_id) VALUES (@FirstName, @LastName, @Id, @ClassId) RETURNING id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Id = entity.id,
                    ClassId = entity.classId
                });
                return result;
            }
        }
    }
}
