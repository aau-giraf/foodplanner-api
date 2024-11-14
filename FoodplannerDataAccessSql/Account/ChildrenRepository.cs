
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
        SELECT 
            children.first_name AS FirstName,
            children.last_name AS LastName,
            classroom.class_name AS ClassName,
            children.child_id AS ChildId,
            classroom.class_id AS ClassId
        FROM 
            children
        JOIN 
            users ON children.parent_id = users.id
        JOIN 
            classroom ON children.class_id = classroom.class_id
        WHERE 
            users.role_approved = 'true';";
            
            using (var connection = _connectionFactory.Create()){
                var children = await connection.QueryAsync<ChildrenGetAllDTO>(query);
                return children;

            }
        
        }
        
        public async Task<int> GetParentIdByChildIdAsync(int id)
        {
            var sql = "SELECT parent_id FROM children WHERE child_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<Children> GetByParentIdAsync(int id)
        {
            var sql = "SELECT * FROM children WHERE parent_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<Children>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> InsertAsync(Children entity)
        {
            var sql = "INSERT INTO children (first_name, last_name, parent_id, class_id) VALUES (@FirstName, @LastName, @ParentId, @ClassId) RETURNING child_id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    ParentId = entity.parentId,
                    ClassId = entity.classId
                });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Children entity)
        {
            var sql = "UPDATE children SET first_name = @FirstName, last_name = @LastName, parent_id = @ParentId, class_id = @ClassId WHERE child_id = @ChildId";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new
                {
                    ChildId = entity.ChildId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    ParentId = entity.parentId,
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
    }
}
