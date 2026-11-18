using System.ComponentModel;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public class CategoriesService()
{
// create category
public async Task<Categories> CreateCategory(Categories categories)
    {
        if (categories.Name == null)
        {
            throw new Exception("Category must have a name");
        }

      return await categoriesRepository.InsertAsync(categories);
    }
}