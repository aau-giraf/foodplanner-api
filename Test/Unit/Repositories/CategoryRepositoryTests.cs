using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Tags;
using Moq;

namespace Test.Unit.RepositoryTests;

public class CategoryRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecuter = new();

    private readonly CategoryRepository repository;

    public CategoryRepositoryTests()
    {
        repository = new CategoryRepository(mockExecuter.Object);
    }
}