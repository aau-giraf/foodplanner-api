
using FoodplannerApi.Controller;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    public async Task GetAll_ShouldReturnCorrectSubIngredients()
    {
        // Arrange
        int parentId = 42;
        var expected1 = CreateSubIngredient(parentId, 1, 1);
        var expected2 = CreateSubIngredient(parentId, 2, 2);

        var expectedSubIngredients = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedSubIngredients);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var subIngredients = Assert.IsAssignableFrom<IEnumerable<SubIngredientDTO>>(okResult.Value).ToList();

        Assert.Equal(2, subIngredients.Count);
        Assert.Equal(expected1.Parent_id, subIngredients[0].Parent_id);
        Assert.Equal(expected1.Child_id, subIngredients[0].Child_id);
        Assert.Equal(expected1.order_no, subIngredients[0].order_no);

        Assert.Equal(expected2.Parent_id, subIngredients[1].Parent_id);
        Assert.Equal(expected2.Child_id, subIngredients[1].Child_id);
        Assert.Equal(expected2.order_no, subIngredients[1].order_no);

        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
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
