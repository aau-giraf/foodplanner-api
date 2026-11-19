using FoodplannerModels.Tags;

namespace FoodplannerModels.Tags;
public interface ICategoryService
{
    Task<int> CreateCategory(CreateCategoryDTO createCategory);
}