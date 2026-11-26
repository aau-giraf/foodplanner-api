namespace Test.E2E;

[CollectionDefinition("E2EDatabase")]
public class E2EDatabaseCollection: ICollectionFixture<DatabaseFixture>,ICollectionFixture<E2EWebApplicationFactory>{}