
using Dapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;

namespace FoodplannerDataAccessSql.Account
{
    public class ChildrenRepository : GenericRepository<Children>, IChildrenRepository
    {

        private readonly PostgreSQLConnectionFactory _connectionFactory;

        public ChildrenRepository(PostgreSQLConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
            _connectionFactory = connectionFactory;
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

            using (var connection = _connectionFactory.Create())
            {
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
        
        public async Task<int> GetChildIdByParentIdAsync(int id)
        {
            var sql = "SELECT child_id FROM children WHERE parent_id = @Id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return result;
            }
        }
    }
}
