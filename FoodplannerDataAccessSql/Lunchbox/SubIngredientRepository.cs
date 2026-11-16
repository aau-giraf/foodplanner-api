using Dapper;
using FoodplannerModels.Lunchbox;

// Adds the SubIngredientRepository to the FoodplannerDataAccessSql.Lunchbox namespace
namespace FoodplannerDataAccessSql.Lunchbox;

// Repository for the SubIngredient class.
public class SubIngredientRepository(IDapperExecutor executor) : ISubIngredientRepository
{
    // Get all packed ingredients from the database. Used in testing.
    public async Task<IEnumerable<SubIngredient>> GetAllAsync()
    {
        var sql = "SELECT * FROM subingredient";
        return await executor.QueryAsync<SubIngredient>(sql);
        
    }

    // Get all subingredients associated with parent id
    public async Task<IEnumerable<SubIngredient>> GetAllByParentIdAsync(int id)
    {
        var sql = "SELECT * FROM subingredient WHERE parent_id = @Id ORDER BY order_no";
        return await executor.QueryAsync<SubIngredient>(sql, new { Id = id });
    }

    // Inserts a new subingredient into the database
    public async Task<int> InsertAsync(SubIngredient entity)
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
    public async Task<int> UpdateAsync(SubIngredient Old, SubIngredient New)
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
    public async Task<int> DeleteAsync(SubIngredient entity)
    {
        var sql = "DELETE FROM subingredient WHERE parent_id = @ParentId AND child_id = @ChildId";
        return await executor.ExecuteAsync(sql, new
        {
            ParentId = entity.Parent_id,
            ChildId = entity.Child_id
        });
    }
}