namespace testing;

[Collection("Database collection")]
public class IngredientRepositoryTest(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _fixture = fixture;

    [Fact]
    public void TestIngredientRepositoryMethod()
    {
        // Test logic here
    }
}