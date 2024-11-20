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
            var sql = "SELECT * FROM packed_ingredients";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                return await connection.QueryAsync<PackedIngredient>(sql);
            }
        }

        // Get all packed ingredients by a meal ID
        public async Task<IEnumerable<PackedIngredient>> GetAllByMealIdAsync(int id) {
            var sql = $"SELECT * FROM packed_ingredients WHERE meal_ref = '{id}'";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open(); 
                var result = await connection.QueryAsync<PackedIngredient>(sql);
                if (result == null) return null;
                else return result;
            }
        }

        // Get a specific packed ingredient by its ID
        public async Task<PackedIngredient> GetByIdAsync(int id) {
            var sql = $"SELECT * FROM packed_ingredients WHERE id = '{id}'";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open(); 
                var result = await connection.QuerySingleOrDefaultAsync<PackedIngredient>(sql, new { Id = id });
                if (result == null) return null;
                else return result;
            }
        }

        // Inserts a new packed ingredient into the database and returns its Id
        public async Task<int> InsertAsync(int meal_ref, int ingredient_ref) {
            var sql = $"INSERT INTO packed_ingredients (meal_ref, ingredient_ref) VALUES ('{meal_ref}', '{ingredient_ref}') RETURNING id";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                return await connection.QuerySingleAsync<int>(sql, new
                {
                    Meal_ref = meal_ref,
                    Ingredient_ref = ingredient_ref
                });
            }
        }


        // Updates an existing packed ingredient
        public async Task<int> UpdateAsync(PackedIngredient entity, int id) {
            var sql = $"UPDATE packed_ingredients SET meal_ref = @Meal_ref, ingredient_ref = @Ingredient_ref WHERE id = '{id}'";
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                return await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    Meal_ref = entity.Meal_ref,
                    Ingredient_ref = entity.Ingredient_ref
                });
            }
        }

        // Deletes a packed ingredient from the database by its ID
        public async Task<int> DeleteAsync(int id) {
            var sql = $"DELETE FROM packed_ingredients WHERE id = '{id}'"; 
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                return await connection.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}