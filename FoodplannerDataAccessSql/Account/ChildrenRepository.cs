using FoodplannerModels.Account;
using Dapper;

namespace FoodplannerDataAccessSql.Account;
 
public class ChildrenRepository : IChildrenRepository
{
    // Setup database connection
    private readonly CorePostgreSQLConnectionFactory _connectionFactory;
    public ChildrenRepository(CorePostgreSQLConnectionFactory connectionFactory)
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
    
    // Given a child ID, find their parents and retrieve those users (parents) from the database 
    // Using placeholder @ to match id's 
    public async Task<IEnumerable<User>> GetParentsByChildIdAsync(int childId)
    {
        var sql = @"SELECT u.* FROM users u
                    JOIN child_relation cr ON u.id = cr.user_id
                    WHERE cr.child_id = @ChildId AND u.role = 'Parent'";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<User>(sql, new { ChildId = childId });
            return result;
        }
    }

    // Given parent id, find their children by Id, and retrieve them from the database 
    public async Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId)
    {
        var sql = @"SELECT c.*
                    FROM children c
                    JOIN child_relation cr ON c.child_id = cr.child_id
                    WHERE cr.user_id = @ParentId";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Children>(sql, new { ParentId = parentId });
            return result;
        }
    }

            // Given child id, find their teachers 
    // by finding 'user' entries in child_relation table, where the 'user' is a teacher  
    public async Task<IEnumerable<User>> GetTeachersByChildIdAsync(int childId)
    {
        var sql = @"SELECT u.* FROM users u
                    JOIN child_relation cr ON u.id = cr.user_id
                    WHERE cr.child_id = @ChildId AND u.role = 'Teacher'";
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
                    JOIN child_relation cr ON c.child_id = cr.child_id
                    JOIN users u ON cr.user_id = u.id
                    WHERE cr.user_id = @TeacherId AND u.role = 'Teacher'";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Children>(sql, new { TeacherId = teacherId });
            return result;
        }
    }

    // Add user-child relation to child_relation table in database given user id and child id 
    public async Task<int> AddUserToChildAsync(int userId, int childId)
    {
        var sql = "INSERT INTO child_relation (user_id, child_id) VALUES (@UserId, @ChildId) ON CONFLICT DO NOTHING";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { UserId = userId, ChildId = childId });
            return result;
        }
    }

    // Remove user-child relation from child_relation table in database given user id and child id 
    public async Task<int> RemoveUserFromChildAsync(int userId, int childId)
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