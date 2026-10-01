
using Dapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{
    // Handles child accounts in the database 
    public class ChildrenRepository : IChildrenRepository
    {

        private readonly PostgreSQLConnectionFactory _connectionFactory;

        // Constructor 
        public ChildrenRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Retrieve all child accounts from database 
        public async Task<IEnumerable<Children>> GetAllAsync()
        {
            var sql = "SELECT * FROM children";
            using (var connection = _connectionFactory.Create())
            {
                var children = await connection.QueryAsync<Children>(sql);
                return children;

            }

        }

        // Select children (name and ID) and their classrooms (name and ID)
        // To get the parents of children in a classroom 
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

        // Given a child ID, find their parents and retrieve those users (parents) from the database 
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

        // Given parent id, find their children, and retrieve them from the database 
        public async Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId)
        {
            var sql = @"SELECT child_id FROM child_relation c
                       WHERE c.user_id = @ParentId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<Children>(sql, new { ParentId = parentId });
                return result;
            }
        }

        // Add a new child to the database 
        public async Task<int> InsertAsync(Children entity)
        {
            var sql = "INSERT INTO children (child_id, first_name, last_name, class_id) VALUES (@child_id, @FirstName, @LastName, @ClassId) RETURNING child_id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    child_id = entity.ChildId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    ClassId = entity.ClassId
                });
                return result;
            }
        }

        // Update a child entry (first name, last name, and its class id) in the database 
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
                    ClassId = entity.ClassId
                });
                return result;
            }
        }

        // Delete a child entry in the database based on child id 
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

        // Retrieve a child entry from database based on child id 
        public async Task<Children?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM children WHERE child_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<Children>(sql, new { Id = id });
                return result;
            }
        }
        
        // Add child-parent relation to child_relation table in database given parent id and child id 
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

        // Remove child-parent relation from child_relation table in database given parent id and child id 
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

        // Add a child-teacher relation in child_relation table in database given teacher id and child id 
        public async Task<int> AddTeacherToChildAsync(int userId, int childId)
        {
            var sql = "INSERT INTO child_relation (user_id, child_id) VALUES (@UserId, @ChildId) ON CONFLICT DO NOTHING";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { UserId = userId, ChildId = childId });
                return result;
            }
        }

        // Remove child-teacher relation in child_relation table in database given teacher id and child id 
        public async Task<int> RemoveTeacherFromChildAsync(int userId, int childId)
        {
            var sql = "DELETE FROM child_relation WHERE user_id = @UserId AND child_id = @ChildId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { UserId = userId, ChildId = childId });
                return result;
            }
        }

        // Given child id, find their teachers 
        // by finding 'user' entries in child_relation table, where the 'user' is a teacher  
        public async Task<IEnumerable<User>> GetTeachersByChildIdAsync(int childId)
        {
            var sql = @"SELECT u.* FROM users u
                       JOIN child_relation uc ON u.id = uc.user_id
                       WHERE uc.child_id = @ChildId AND u.role = 'Teacher'";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql, new { ChildId = childId });
                return result;
            }
        }

        // Given teacher id, find their children 
        // by retrieving child entries in child_relation table, where the 'user' is a teacher  
        public async Task<IEnumerable<Children>> GetChildrenByTeacherIdAsync(int teacherId)
        {
            var sql = @"SELECT c.* FROM children c
                       JOIN child_relation uc ON c.child_id = uc.child_id
                       WHERE uc.user_id = @TeacherId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QueryAsync<Children>(sql, new { TeacherId = teacherId });
                return result;
            }
        }
    }
}
