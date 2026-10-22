using Dapper;
using FoodplannerModels.Lunchbox;

// Adds the SubIngredientRepository to the FoodplannerDataAccessSql.Lunchbox namespace
namespace FoodplannerDataAccessSql.Lunchbox;

// Repository for the SubIngredient class.
public class SubIngredientRepository(IDapperExecutor executor) : ISubIngredientRepository
{
    // Get all packed ingredients from the database. Used in testing.
    public async Task<IEnumerable<SubIngredientDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM subingredient";
        return await executor.QueryAsync<SubIngredientDTO>(sql);
        
    }

    // Get all subingredients associated with parent id
    public async Task<IEnumerable<SubIngredientDTO>> GetAllByParentIdAsync(int id)
    {
        var sql = "SELECT * FROM subingredient WHERE parent_id = @Id ORDER BY order_no";
        return await executor.QueryAsync<SubIngredientDTO>(sql, new { Id = id });
    }

    // Inserts a new subingredient into the database
    public async Task<int> InsertAsync(SubIngredientDTO entity)
    {
        var sql = "INSERT INTO subingredient (parent_id, child_id, order_no) VALUES (@ParentId, @ChildId, @OrderNumber)";
        return await executor.ExecuteAsync(sql, new
            {
                ParentId = entity.Parent_id,
                ChildId = entity.Child_id,
                OrderNumber = entity.order_no
            });
    }

    // Updates an existing subingredient
    public async Task<int> UpdateAsync(SubIngredientDTO Old, SubIngredientDTO New)
    {
        var sql = "UPDATE subingredient SET parent_id = @NewParentId, child_id = @NewChildId, order_no = @NewOrderNumber WHERE parent_id = @OldParentId AND child_id = @OldChildId";
        return await executor.ExecuteAsync(sql, new
        {
            NewParentId = New.Parent_id,
            NewChildId = New.Child_id,
            NewOrderNumber = New.order_no,
            OldParentId = Old.Parent_id,
            OldChildId = Old.Child_id
        });
    }

    // Deletes a subingredient from the database
    public async Task<int> DeleteAsync(SubIngredientDTO entity)
    {
        var sql = "DELETE FROM subingredient WHERE parent_id = @ParentId AND child_id = @ChildId";
        return await executor.ExecuteAsync(sql, new
        {
            ParentId = entity.Parent_id,
            ChildId = entity.Child_id
        });
    }
}