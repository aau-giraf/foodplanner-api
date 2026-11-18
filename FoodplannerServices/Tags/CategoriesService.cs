using System.ComponentModel;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public class CategoriesService(ICategoryRepository categoriesRepository)
{
// create category
public async Task<CategoryDTO> CreateCategory(CategoryDTO category)
    {
        if (category.Name == null)
        {
            throw new Exception("Category must have a name");
        }

      return await categoriesRepository.InsertAsync(category);
    }
}