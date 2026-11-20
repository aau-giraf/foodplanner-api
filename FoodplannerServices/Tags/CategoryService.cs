using System.ComponentModel;
using System.Linq.Expressions;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public class CategoryService(ICategoryRepository categoryRepository)
{
// Create a category in the repository
public async Task<CategoryDTO> CreateCategory(CategoryDTO categories)
    {
       try
        {
            return await categoryRepository.InsertAsync(categories);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error creating category: {exception.Message}", exception);

        }
    }
}