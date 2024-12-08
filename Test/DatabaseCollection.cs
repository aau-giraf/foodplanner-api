namespace Test.Repository;

/** Method for connecting multiple tests to the DatabaseFixture class.
/* The CollectionDefinition tells the compiler that every test class that
/* is part of this collection should initialize the ICollectionFixture
/* once and share it between all the test classes.
/* This way, the connection to the database only need to be made once
/* durring the tests.
*/
[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code; it’s just a marker for the collection definition.
}