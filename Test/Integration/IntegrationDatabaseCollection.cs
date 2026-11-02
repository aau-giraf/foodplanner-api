using Xunit;

namespace Test.Integration
{
    [CollectionDefinition("IntegrationDatabase")]
    public class IntegrationDatabaseCollection
        : ICollectionFixture<DatabaseFixture>
    {
    }
}