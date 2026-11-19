
using FoodplannerApi.Controller;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FoodplannerModels.Account;

namespace Test.Integration.Controllers;

public class SubIngredientsTests
{
    private readonly Mock<ISubIngredientRepository> mockRepository = new();
    private readonly Mock<IAuthService> mockAuthService = new();
    private readonly SubIngredientsController controller;

    public SubIngredientsTests()
    {
        var service = new SubIngredientService(mockRepository.Object);
        controller = new SubIngredientsController(service, mockAuthService.Object);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAll_ShouldReturnCorrectSubIngredientsById()
    {
        // Arrange
        var parentId = 1;
        var expected = CreateSubIngredient(parentId, 2, 1);

        mockRepository.Setup(r => r.GetAllByParentIdAsync(parentId)).ReturnsAsync(new[] { expected });

        // Act
        var result = await controller.Get(parentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var subIngredients = Assert.IsAssignableFrom<IEnumerable<SubIngredientDTO>>(okResult.Value).ToList();
        var subIngredient = Assert.Single(subIngredients);

        Assert.Equal(expected.Parent_id, subIngredient.Parent_id);
        Assert.Equal(expected.Child_id, subIngredient.Child_id);
        Assert.Equal(expected.order_no, subIngredient.order_no);

        mockRepository.Verify(r => r.GetAllByParentIdAsync(parentId), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task Create_ShouldCreateSubIngredient()
    {
        // Arrange
        int userId = 42;
        string token = "Bearer test-token";
        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        var expected = CreateSubIngredient(userId, 2, 1);

        mockRepository.Setup(r => r.InsertAsync(expected)).ReturnsAsync(1);

        // Act
        var result = await controller.Create(token, expected);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var affectedRows = Assert.IsType<int>(okResult.Value);

        Assert.Equal(1, affectedRows);

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(token), Times.Once);
        mockRepository.Verify(r => r.InsertAsync(expected), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task Update_ShouldUpdateSubIngredient()
    {
        // Arrange
        int userId = 42;
        int childId = 4;
        var updated = CreateSubIngredient(userId, childId, 1);

        mockRepository.Setup(r => r.UpdateAsync(It.Is<SubIngredientDTO>(s =>s.Parent_id == userId && s.Child_id == childId && s.order_no == 0),updated)).ReturnsAsync(1);

        // Act
        var result = await controller.Update(updated, userId, childId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var affectedRows = Assert.IsType<int>(okResult.Value);

        Assert.Equal(1, affectedRows);

        mockRepository.Verify(r => r.UpdateAsync(
            It.Is<SubIngredientDTO>(s =>
                s.Parent_id == userId &&
                s.Child_id == childId &&
                s.order_no == 0),
            updated), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task Delete_ShouldDeleteSubIngredient()
    {
        // Arrange
        int userId = 42;
        int childId = 4;
        var deleted = CreateSubIngredient(userId, childId, 1);

        mockRepository.Setup(r => r.DeleteAsync(deleted)).ReturnsAsync(1);

        // Act
        var result = await controller.Delete(deleted);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var affectedRows = Assert.IsType<int>(okResult.Value);

        Assert.Equal(1, affectedRows);

        mockRepository.Verify(r => r.DeleteAsync(deleted), Times.Once);
    }

    private static SubIngredientDTO CreateSubIngredient(int parentId, int childId, int orderNo)
    {
        return new SubIngredientDTO
        {
            Parent_id = parentId,
            Child_id = childId,
            order_no = orderNo
        };
    }
}
