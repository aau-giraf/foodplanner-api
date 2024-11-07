using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerDataAccessSql.Account;
using FoodplannerApi.Controller;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using FoodplannerModels.Account;

namespace testing;

/**
* These are technicly integration tests, since the database has not been mocked using Xunits Fixture.
* It is therefor important to check that the database SW5-10 is empty before running the tests.
*/
[Collection("Sequential")]
public class IngredientsControllerTest
{
    private static readonly UserRepository userRep = new(DatabaseConnection.GetConnection());
    private static readonly IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
    private static readonly IngredientService ingredientServ = new(ingredientRep);
    private static readonly AuthService authServ = new(WebApplication.CreateBuilder().Configuration);
    private static readonly IngredientsController ingredientsCon = new(ingredientServ, authServ);

    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("ingredients");

        //Attempt
        IActionResult actual = await ingredientsCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);

    }

    [Fact]
    public async void GetAllByUser_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("ingredients");
        User user = new() {Id = 0, FirstName = "test", LastName = "test", Email = "", Password = "", Role = "Parent", RoleApproved = true, PinCode = ""};

        //Attempt
        IActionResult actual = await ingredientsCon.GetAllByUser("Bearer " + authServ.GenerateJWTToken(user));

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void GetAllByUser_TwoIngredients_ReturnsOkObjectResultOfOneIngredient()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("ingredients");
        User user2 = new() {Id = 2, FirstName = "test2", LastName = "test2", Email = "", Password = "", Role = "Parent", RoleApproved = true, PinCode = ""};
        Ingredient ingredient1 = new() { Id = 0, Name = "test1", User_ref = 1, Image_ref = 1};
        Ingredient ingredient2 = new() { Id = 0, Name = "test2", User_ref = 2, Image_ref = 1};

        //Attempt
        await userRep.InsertAsync(user2);
        await ingredientRep.InsertAsync(ingredient1);
        await ingredientRep.InsertAsync(ingredient2);
        IActionResult action = await ingredientsCon.GetAllByUser("Bearer " + authServ.GenerateJWTToken(user2));
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

        //Attempt
        IActionResult actual = await ingredientsCon.Get(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }


        [Fact]
    public async void Get_CorrectInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("ingredients");
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        IActionResult actual = await ingredientsCon.Get(ingredientId);

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
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        IActionResult actual = await ingredientsCon.Create(ingredient);

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
        Ingredient ingredient = new() { Id = 0, Name = "old test", User_ref = 1, Image_ref = 1};
        Ingredient updatedIngredient = new() { Id = 0, Name = "new test", User_ref = 1, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        IActionResult actual = await ingredientsCon.Update(updatedIngredient, ingredientId);

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
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        IActionResult actual = await ingredientsCon.Update(ingredient, 0);

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
        Ingredient ingredient = new() { Id = 0, Name = "test", User_ref = 1, Image_ref = 1};

        //Attempt
        await ingredientRep.InsertAsync(ingredient);
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int ingredientId = allIngredients.FirstOrDefault().Id;
        IActionResult actual = await ingredientsCon.Delete(ingredientId);

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

        //Attempt
        IActionResult actual = await ingredientsCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }
}