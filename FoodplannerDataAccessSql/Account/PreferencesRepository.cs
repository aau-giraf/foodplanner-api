using FoodplannerModels.Preferences;

namespace FoodplannerDataAccessSql.Account;

// Adds Preference Repository to the FoodplannerDataAccessSql.Account namespace
public class PreferencesRepository(IDapperExecutor executor) : IPreferencesRepository
{
    // Method to insert a new preference into preference table
    // Preference contains: child's id, their rating, food name, date and the id of the food image
    // Returns an ID
    public async Task<int> InsertAsync(Preferences entity)
    {
        var sql = "INSERT INTO preferences (child_id, rating, food_name, date, food_image_id) VALUES (@Child_id, @Rating, @Food_name, @Date, @Food_image_id) RETURNING id";

        return await executor.QuerySingleAsync<int>(sql, new
        {
            entity.Child_id,
            entity.Rating,
            entity.Food_name,
            entity.Date,
            entity.Food_image_id
        });
    }

    // Method for updating the preferences
    // Returns ID of preference
    public async Task<int> UpdateAsync(Preferences entity)
    {
        var sql = "UPDATE preferences SET rating = @Rating, food_name = @Food_name, date = @Date, food_image_id = @Food_image_id WHERE id = @Id RETURNING id";

        return await executor.QuerySingleAsync<int>(sql, new
        {
            entity.Id,
            entity.Rating,
            entity.Food_name,
            entity.Date,
            entity.Food_image_id
        });
    }

    // Retrieves previously disliked foods that are eligible to be tried again
    public async Task<IEnumerable<Preferences>> GetRetryCandidatesAsync(
        int childId,
        string dislikedRating,
        DateTime cutoffDate,
        int limit)
    {
        var sql = """
            WITH latest_preferences AS (
                SELECT DISTINCT ON (LOWER(TRIM(food_name)))
                    id AS "Id",
                    child_id AS "Child_id",
                    food_image_id AS "Food_image_id",
                    food_name AS "Food_name",
                    rating AS "Rating",
                    date AS "Date"
                FROM preferences
                WHERE child_id = @ChildId
                ORDER BY LOWER(TRIM(food_name)), date DESC, id DESC
            )
            SELECT *
            FROM latest_preferences
            WHERE LOWER(TRIM("Rating")) = LOWER(@DislikedRating)
                AND "Date" < @CutoffDate
            ORDER BY "Date" ASC
            LIMIT @Limit;
            """;

        return await executor.QueryAsync<Preferences>(sql, new
        {
            ChildId = childId,
            DislikedRating = dislikedRating,
            CutoffDate = cutoffDate,
            Limit = limit
        });
    }
}