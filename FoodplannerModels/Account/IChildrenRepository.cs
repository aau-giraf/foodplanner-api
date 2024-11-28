namespace FoodplannerModels.Account
{
    public interface IChildrenRepository
    {
        Task<IEnumerable<Children>> GetAllAsync();
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync();
        Task<Children> GetByParentIdAsync(int id);
        Task<int> InsertAsync(Children entity);
        Task<int> GetParentIdByChildIdAsync(int id);
        Task<int> UpdateAsync(Children entity);

        Task<int> DeleteAsync(int id);
    }
}
