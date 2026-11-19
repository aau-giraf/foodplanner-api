// Adds Interface CategoryService to the class to the FoodplannerServices.Tags namespace
namespace FoodplannerModels.Tags;

// Interface for Category Service 
public interface ICategoryService
{
    Task<int> CreateCategory(CreateCategoryDTO createCategory);
}