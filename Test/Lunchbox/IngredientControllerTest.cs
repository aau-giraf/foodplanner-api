using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;

namespace testing;

[Collection("Sequential")]
public class IngredientsControllerTest
{
    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);

    }

    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.Get(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }


        [Fact]
    public async void Get_CorrectInput_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};

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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};

        //Attempt
        IActionResult actual = await ingredientCon.Create(ingredient);

        //Verify
        Assert.IsType<CreatedAtActionResult>(actual);
    }


[Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "old", Image_ref = "test"};
        Ingredient updatedIngredient = new Ingredient{Id = 0, Name = "test", User_ref = "new", Image_ref = "test"};

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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};

        //Attempt
        IActionResult actual = await ingredientCon.Update(ingredient, 0);

        //Verify
        Assert.IsType<BadRequestResult>(actual);
    }

    
    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);
        Ingredient ingredient = new Ingredient{Id = 0, Name = "test", User_ref = "test", Image_ref = "test"};

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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("ingredients");
        IngredientRepository ingredientRep = new IngredientRepository(DatabaseConnection.GetConnection());
        IngredientService ingredientServ = new IngredientService(ingredientRep);
        IngredientsController ingredientCon = new IngredientsController(ingredientServ);

        //Attempt
        IActionResult actual = await ingredientCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }
}