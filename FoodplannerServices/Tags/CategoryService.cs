using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;

// Adds CategoryService to the class to the FoodplannerServices.Tags namespace
namespace FoodplannerServices.Tags;

// Service implementation for Category business logic
public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
 
    // Method for creating a category in the repository
    public async Task<int> CreateCategoryAsync(CreateCategoryDTO createCategory)
        {
        var category = new Category
        {
            Name = createCategory.Name,
            Main = createCategory.Main
        };

        // Checks if category already exists
        if (await categoryRepository.CategoryExistsAsync(category.Name))
        {
            throw new InvalidOperationException("Kategori eksisterer allerede");
        }

        // Inserts category into database
        return await categoryRepository.InsertAsync(category);
    }
}