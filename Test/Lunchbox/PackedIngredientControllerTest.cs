using FoodplannerApi.Controller;
using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace testing
{
    public class PackedIngredientControllerIntegrationTests
    {
        // private readonly PackedIngredientRepository _repository;
        // private readonly PackedIngredientService _service;
        // private readonly PackedIngredientController _controller;

        // public PackedIngredientControllerIntegrationTests()
        // {
        //     // Brug en reel databaseforbindelse
        //     var connectionFactory = DatabaseConnection.GetConnection(); 
        //     _repository = new PackedIngredientRepository(connectionFactory);
        //     _service = new PackedIngredientService(_repository);
        //     _controller = new PackedIngredientController(_service);
        // }

        // [Fact]
        // public async Task GetAll_ReturnsOkResult_WithListOfPackedIngredients()
        // {
        //     // Act
        //     var result = await _controller.GetAll();

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var packedIngredients = Assert.IsType<List<PackedIngredient>>(okResult.Value);
        //     Assert.True(packedIngredients.Any());  // Ensure there's at least one packed ingredient
        // }

        // [Fact]
        // public async Task Get_ReturnsOkResult_WhenPackedIngredientExists()
        // {
        //     // Arrange
        //     int validId = 1; // Assume this ID exists in the test database

        //     // Act
        //     var result = await _controller.Get(validId);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var packedIngredient = Assert.IsType<PackedIngredient>(okResult.Value);
        //     Assert.Equal(validId, packedIngredient.Id);
        // }

        // [Fact]
        // public async Task Get_ReturnsNotFound_WhenPackedIngredientDoesNotExist()
        // {
        //     // Arrange
        //     int invalidId = 9999; // Assume this ID does not exist

        //     // Act
        //     var result = await _controller.Get(invalidId);

        //     // Assert
        //     Assert.IsType<NotFoundResult>(result);
        // }

        // [Fact]
        // public async Task Create_ReturnsCreatedAtAction_WhenPackedIngredientIsCreated()
        // {
        //     // Arrange
        //     var newPackedIngredient = new PackedIngredient { Id = 5, Meal_ref = 19, Ingredient_ref = 3 };

        //     // Act
        //     var result = await _controller.Create(newPackedIngredient);

        //     // Assert
        //     var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        //     var createdPackedIngredient = Assert.IsType<PackedIngredient>(createdAtActionResult.Value);
        //     Assert.True(createdPackedIngredient.Id > 0);  // Ensure the new ingredient has a valid ID
        // }

        // [Fact]
        // public async Task Update_ReturnsOkResult_WhenUpdateIsSuccessful()
        // {
        //     // Arrange
        //     var existingPackedIngredient = new PackedIngredient { Id = 7, Meal_ref = 18, Ingredient_ref = 3 };
        //     int validId = 7;  // Assume this ID exists in the database

        //     // Act
        //     var result = await _controller.Update(existingPackedIngredient, validId);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var updatedPackedIngredient = Assert.IsType<PackedIngredient>(okResult.Value);
        //     Assert.Equal(validId, updatedPackedIngredient.Id);
        //     Assert.Equal(3, updatedPackedIngredient.Ingredient_ref);  // Ensure ingredient_ref is updated
        // }


        // [Fact]
        // public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
        // {
        //     // Arrange
        //     int validId = 7; // Assume this ID exists and can be deleted

        //     // Act
        //     var result = await _controller.Delete(validId);

        //     // Assert
        //     Assert.IsType<NoContentResult>(result);
        // }

        // [Fact]
        // public async Task Delete_ReturnsNotFound_WhenDeleteFails()
        // {
        //     // Arrange
        //     int invalidId = 9999; // Assume this ID does not exist

        //     // Act
        //     var result = await _controller.Delete(invalidId);

        //     // Assert
        //     Assert.IsType<NotFoundResult>(result);
        // }
    }
}

