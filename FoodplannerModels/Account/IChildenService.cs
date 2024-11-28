namespace FoodplannerModels.Account
{
    public interface IChildrenService
    {
        Task<IEnumerable<Children>> GetAllChildrenAsync();
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync();
        Task<Children> GetChildrenByIdAsync(int id);
        Task<int> CreateChildrenAsync(ChildrenCreateParentDTO children);
        Task<int> UpdateChildrenAsync(Children children);

        Task<int> DeleteChildrenAsync(int id);
    }
}
