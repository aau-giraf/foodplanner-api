using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace testing;

/**
* These are technicly integration tests, since the database has not been mocked using Xunits Fixture.
* It is therefor important to check that the database SW5-10 is empty before running the tests.
*/
[Collection("Sequential")]
public class IngredientsControllerTest
{
    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        //Setup
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
    public async void GetAllByUser_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.GetAllByUser(1);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void GetAllByUser_TwoIngredients_ReturnsOkObjectResultOfOneIngredient()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        //--New User--
        using var connection = DatabaseConnection.GetConnection().Create();
        await connection.OpenAsync();
        var sql = "INSERT INTO users (first_name, last_name, email, password, role, role_approved)\n";
        sql += $"VALUES ('Temp2', 'Temp2', 'empty', '1234', 'Test', true)";
        await connection.ExecuteAsync(sql);
        //--New User--
        Ingredient ingredient1 = new() { Id = 0, Name = "test1", User_ref = 1, Image_ref = 1};
        Ingredient ingredient2 = new() { Id = 0, Name = "test2", User_ref = 2, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient1);
        await ingredientRep.InsertAsync(ingredient2);
        IActionResult action = await ingredientCon.GetAllByUser(2);
        var objectResult = action as OkObjectResult;
        var actual = objectResult.Value as List<Ingredient>;

        //Clean Up
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(action);
        Assert.Equal(ingredient2.Name, actual[0].Name);
    }

    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        //Setup
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

        //Clean Up
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Create_CorrectInput_ReturnsCreatedAtActionResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        IActionResult actual = await ingredientCon.Create(ingredient);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<CreatedAtActionResult>(actual);
    }


[Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
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

        //Clean Up
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }


        [Fact]
    public async void Update_WrongId_ReturnsBadRequestResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        IActionResult actual = await ingredientCon.Update(ingredient, 0);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<BadRequestResult>(actual);
    }

    
    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
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

        //Clean Up
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }


    [Fact]
    public async void Delete_WrongId_ReturnsNotFounfResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new(ingredientRep);
        IngredientsController ingredientCon = new(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }
}