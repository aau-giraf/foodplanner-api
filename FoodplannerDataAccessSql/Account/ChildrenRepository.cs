
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
            var sql = "SELECT * FROM children";
            using (var connection = _connectionFactory.Create())
            {
                var children = await connection.QueryAsync<Children>(sql);
                return children;

            }

        }

        public async Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync()
        {
            var query = @"
        SELECT DISTINCT
            children.first_name AS FirstName,
            children.last_name AS LastName,
            classroom.class_name AS ClassName,
            children.child_id AS ChildId,
            classroom.class_id AS ClassId
        FROM 
            children
        JOIN 
            child_relation ON children.child_id = child_relation.child_id
        JOIN 
            users ON child_relation.user_id = users.id
        JOIN 
            classroom ON children.class_id = classroom.class_id
        WHERE 
            users.role_approved = 'true';";

            using (var connection = _connectionFactory.Create())
            {
                var children = await connection.QueryAsync<ChildrenGetAllDTO>(query);
                return children;

            }

        }

        public async Task<IEnumerable<User>> GetParentsByChildIdAsync(int childId)
        {
            var sql = @"SELECT u.* FROM users u
                       JOIN child_relation uc ON u.id = uc.user_id
                       WHERE uc.child_id = @ChildId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql, new { ChildId = childId });
                return result;
            }
        }

        public async Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId)
        {
            var sql = @"SELECT c.* FROM children c
                       JOIN child_relation uc ON c.child_id = uc.child_id
                       WHERE uc.user_id = @ParentId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<Children>(sql, new { ParentId = parentId });
                return result;
            }
        }

        public async Task<int> InsertAsync(Children entity)
        {
            var sql = "INSERT INTO children (first_name, last_name, class_id) VALUES (@FirstName, @LastName, @ClassId) RETURNING child_id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    ClassId = entity.classId
                });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Children entity)
        {
            var sql = "UPDATE children SET first_name = @FirstName, last_name = @LastName, class_id = @ClassId WHERE child_id = @ChildId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new
                {
                    ChildId = entity.ChildId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    ClassId = entity.classId
                });
                return result;
            }
        }


        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM children WHERE child_id = @ChildId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { ChildId = id });
                return result;
            }
        }

        public async Task<Children> GetChildByIdAsync(int id)
        {
            var sql = "SELECT * FROM children WHERE child_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<Children>(sql, new { Id = id });
                return result;
            }
        }
        
        public async Task<int> AddParentToChildAsync(int userId, int childId)
        {
            var sql = "INSERT INTO child_relation (user_id, child_id) VALUES (@UserId, @ChildId) ON CONFLICT DO NOTHING";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { UserId = userId, ChildId = childId });
                return result;
            }
        }

        public async Task<int> RemoveParentFromChildAsync(int userId, int childId)
        {
            var sql = "DELETE FROM child_relation WHERE user_id = @UserId AND child_id = @ChildId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { UserId = userId, ChildId = childId });
                return result;
            }
        }
    }
}
