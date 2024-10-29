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

        // Get all packed ingredients from the database. Used in testing.
        public async Task<IEnumerable<PackedIngredient>> GetAllAsync() {
            var sql = "SELECT * FROM packed_ingredients"; // SQL query to select all packed ingredients
            using var connection = _connectionFactory.Create(); // Creates a new database connection
            connection.Open(); // Opens the database connection
            return await connection.QueryAsync<PackedIngredient>(sql); // Executes the query and returns the results
        }

        // Get all packed ingredients by a meal ID
        public async Task<IEnumerable<PackedIngredient>> GetAllByMealIdAsync(int id) {
            var sql = $"SELECT * FROM packed_ingredients WHERE meal_ref = {id}"; // SQL query to select all packed ingredients by meal ID
            using var connection = _connectionFactory.Create(); 
            connection.Open(); 
            var result = await connection.QueryAsync<PackedIngredient>(sql);
            if (result == null) return null;
            else return result;
        }

        // Get a specific packed ingredient by its ID
        public async Task<PackedIngredient> GetByIdAsync(int id) {
            var sql = $"SELECT * FROM packed_ingredients WHERE id = {id}"; // SQL query to select a packed ingredient by ID
            using var connection = _connectionFactory.Create(); 
            connection.Open(); 
            var result = await connection.QuerySingleOrDefaultAsync<PackedIngredient>(sql, new { Id = id });
            if (result == null) return null;
            else return result;
        }

        // Inserts a new packed ingredient into the database
        public async Task<int> InsertAsync(PackedIngredient entity) {
            using var connection = _connectionFactory.Create();
            connection.Open();

            var sql = $"INSERT INTO packed_ingredients (meal_ref, ingredient_ref)\n";
            sql += $"VALUES ('{entity.Meal_ref}', '{entity.Ingredient_ref}')";

            return await connection.ExecuteAsync(sql, entity);
        }


        // Updates an existing packed ingredient
        public async Task<int> UpdateAsync(PackedIngredient entity, int id) {
            using var connection = _connectionFactory.Create();
            connection.Open();
            var sql = $"UPDATE packed_ingredients\n";
            sql += $"SET meal_ref = '{entity.Meal_ref}', ingredient_ref = '{entity.Ingredient_ref}'";
            sql += $"WHERE id = '{id}'";
            return await connection.ExecuteAsync(sql, entity);
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
    }
}