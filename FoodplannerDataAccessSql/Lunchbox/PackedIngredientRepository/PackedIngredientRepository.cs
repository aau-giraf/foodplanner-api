using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox
{
    /**
    * Repository for the PackedIngredient class.
    */
    public class PackedIngredientRepository (PostgreSQLConnectionFactory connectionFactory) : IPackedIngredientRepository
    {
        private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

        // Get all packed ingredients from the database
        public async Task<IEnumerable<PackedIngredient>> GetAllAsync() {
            var sql = "SELECT * FROM packed_ingredients"; // SQL query to select all packed ingredients
            using var connection = _connectionFactory.Create(); // Creates a new database connection
            connection.Open(); // Opens the database connection
            return await connection.QueryAsync<PackedIngredient>(sql); // Executes the query and returns the results
        }

        // Get a specific packed ingredient by its ID
        public async Task<PackedIngredient> GetByIdAsync(int id) {
            var sql = $"SELECT * FROM packed_ingredients WHERE id = {id}"; // SQL query to select a packed ingredient by ID
            using var connection = _connectionFactory.Create(); 
            connection.Open(); 
            var result = await connection.QueryAsync<PackedIngredient>(sql); 
            return result.FirstOrDefault(); 
        }

        // Inserts a new packed ingredient into the database
        public async Task<int> InsertAsync(PackedIngredient entity) {
            const string sql = "INSERT INTO packed_ingredients (IngredientRef, MealRef) VALUES (@IngredientRef, @MealRef) RETURNING Id";

            using var connection = _connectionFactory.Create();
            await connection.OpenAsync();

            // Execute the insert and return the generated Id
            return await connection.ExecuteScalarAsync<int>(sql, new {
                entity.IngredientRef,
                entity.MealRef
            });
        }


        // Updates an existing packed ingredient (currently not implemented)
        public Task<int> UpdateAsync(PackedIngredient entity) {
            throw new NotImplementedException(); // Placeholder for update functionality
        }

        // Deletes a packed ingredient from the database by its ID
        public async Task<int> DeleteAsync(int id) {
            // SQL query to delete a packed ingredient by ID
            var sql = $"DELETE FROM packed_ingredients WHERE id = {id}"; 
            using var connection = _connectionFactory.Create(); 
            connection.Open(); 

            // Executes the delete query and returns the number of affected rows
            return await connection.ExecuteAsync(sql, new { Id = id });
        }

        // Helper method to generate SQL content dynamically based on the entity's properties
        // If 'properties' is true, it returns the property names, otherwise it returns the values
        private IEnumerable<string> GetContent(PackedIngredient entity, bool properties = true) {
            if (properties) 
                return typeof(PackedIngredient).GetProperties().Select(p => p.Name); 
                // Returns property names for SQL columns
            else 
                return typeof(PackedIngredient).GetProperties().Select(p => "'" + p.GetValue(entity) + "'"); 
                // Returns property values for SQL values
        }
    }
}
