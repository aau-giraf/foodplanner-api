using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace testing;

[Collection("Sequential")]
public class IngredientsControllerTest
{
    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);

    }

    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.Get(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }


        [Fact]
    public async void Get_CorrectInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        IActionResult actual = await ingredientCon.Get(ingredientId);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Create_CorrectInput_ReturnsCreatedAtActionResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        IActionResult actual = await ingredientCon.Create(ingredient);

        //Verify
        Assert.IsType<CreatedAtActionResult>(actual);
    }


[Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "old test", User_ref = 1, Image_ref = 1};
        Ingredient updatedIngredient = new() { Id = 0, Name = "new test", User_ref = 1, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        IActionResult actual = await ingredientCon.Update(updatedIngredient, ingredientId);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }


        [Fact]
    public async void Update_WrongId_ReturnsBadRequestResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        IActionResult actual = await ingredientCon.Update(ingredient, 0);

        //Verify
        Assert.IsType<BadRequestResult>(actual);
    }

    
    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        IActionResult actual = await ingredientCon.Delete(ingredientId);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }


    [Fact]
    public async void Delete_WrongId_ReturnsNotFounfResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async void Z() //The tests are called alphabeticly, so this is called Z to force it to be last
    {
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        Assert.True(true);
    }
}