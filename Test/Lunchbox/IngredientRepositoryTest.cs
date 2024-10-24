using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using Xunit;

namespace testing
{
    public class IngredientRepositoryTests
    {
    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IEnumerable<Ingredient> expected = [];
        
        //Attempt
        IEnumerable<Ingredient> actual = await ingredientRep.GetAllAsync();
        
        //Verify
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void GetAllAsync_OneIngredientInDatabase_ReturnsOneIngredient()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> actual = await ingredientRep.GetAllAsync();
        
        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        
        //Attempt
        Ingredient actual = await ingredientRep.GetByIdAsync(0);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OneIngredientInDatabase_ReturnsTheSameIngredient()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};
        string expected = "test";
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);
        
        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void UpdateAsync_OneIngredientInDatabase_ReturnsTheUpdatedIngredient()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        Ingredient ingredient = new Ingredient{Id = 0, Name = "old", User_ref = "test", Image_ref = "test"};
        Ingredient updatedIngredient = new Ingredient{Id = 0, Name = "new", User_ref = "test", Image_ref = "test"};
        string expected = "new";
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        await ingredientRep.UpdateAsync(updatedIngredient, ingredientId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);
        await ingredientRep.DeleteAsync(ingredientId);
        
        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void DeleteAsync_OneIngredientInDatabase_ReturnsNull()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};
        
        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        await ingredientRep.DeleteAsync(ingredientId);
        Ingredient actual = await ingredientRep.GetByIdAsync(ingredientId);
        
        //Verify
        Assert.Null(actual);
    }
    }
}