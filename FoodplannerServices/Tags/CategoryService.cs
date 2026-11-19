using System.ComponentModel;
using System.Linq.Expressions;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
 
    // Create a category in the repository
    public async Task<int> CreateCategory(CreateCategoryDTO createCategory)
        {
        var category = new CategoryDTO
        {
            Name = createCategory.Name,
            Main = createCategory.Main
        };

        if (await categoryRepository.CategoryExistsAsync(category.Name))
        {
            throw new InvalidOperationException("Kategori eksisterer allerede");
        }

        return await categoryRepository.InsertAsync(category);
    }
}