using System.Collections.Generic;
using Dapper;
using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;

namespace Test.RepositoryTests;

public class ChildrenRepositoryTests
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    // Creates a connection factory for the separate PostgreSQL test database.
    // This prevents repository tests from modifying the normal development database.
    public ChildrenRepositoryTests()
    {
        _connectionFactory = new PostgreSQLConnectionFactory(
            "localhost",
            "7654",
            "giraf_test",
            "postgres",
            "postgres"
        );
    }

    // ----- GET ALL ----- 
    [Fact]
    public async Task GetAllAsync_ReturnChildren()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // Stores the temporary children id's 
        int testChild1, testChild2;  

        // temporary children users are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            testChild1 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name)
                VALUES
                    ('jens', 'jensen')
                RETURNING id
                """
            );

            testChild2 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name)
                VALUES
                    ('søren', 'sørensen')
                RETURNING id
                """
            );
        }
        try
        {
            // Act
            // Retrieve the children users 
            var result = await repository.GetAllAsync();
            var childList = result.ToList(); 

            // Assert
            // Verify that children were returned and values match 
            Assert.NotNull(result);

            bool child1 = childList.Any(c => c.FirstName == "jens" && c.LastName == "jensen" & c.ChildId == testChild1);
            Assert.True(child1);    
            bool child2 = childList.Any(c => c.FirstName == "søren" && c.LastName == "sørensen" & c.ChildId == testChild2);
            Assert.True(child2);    
            
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete the temporary children users 
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChild1 }
            );
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChild2 }
            );
        }
    }

    // ----- GET ALL CHILD W/ CLASSES ----- 
    // Child with Class and parents role_approved = true 
    [Fact]
    public async Task GetAllChildrenClassesAsync_RoleApprovedTrue_ReturnChildrenWithClasses()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // Stores the temporary children id's 
        int testChild1, testChild2, testClassroom, testParent, testRelation1, testRelation2; 

        // temporary data are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            // Temporary classroom 
            testClassroom = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO classroom 
                    (class_name)
                VALUES 
                    ('Math') 
                RETURNING class_id
                """
            );

            // Temporary children users 
            testChild1 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name, class_id)
                VALUES
                    ('anna', 'nielsen', @ClassId)
                RETURNING id
                """,
                new {ClassId = testClassroom}
            );
            testChild2 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name, class_id)
                VALUES
                    ('mette', 'nielsen', @ClassId)
                RETURNING id
                """,
                new {ClassId = testClassroom}
            );

            // Temporary parent
            testParent = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO users 
                    (first_name, last_name, email, password, role, role_approved) 
                VALUES 
                    ('henriette', 'nielsen', 'henriette@nielsen.dk', 'test123', 'Parent', 'true') 
                RETURNING id"
                """
            );

            // Give parent to children
            testRelation1 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO child_relation
                    (user_id, child_id) 
                VALUES (@UserId, @ChildId) 
                ON CONFLICT DO NOTHING"
                """,
                new {UserId = testParent, ChildId = testChild1}
            );
            testRelation2 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO child_relation
                    (user_id, child_id) 
                VALUES (@UserId, @ChildId) 
                ON CONFLICT DO NOTHING"
                """,
                new {UserId = testParent, ChildId = testChild2}
            );            
        }
        try
        {
            // Act
            // Retrieve the children users 
            var result = await repository.GetAllChildrenClassesAsync();
            var list = result.ToList(); 

            // Assert
            // Verify that children were returned and values match 
            Assert.NotNull(result);

            bool child1 = list.Any(c => c.FirstName == "anna" && c.LastName == "nielsen" & c.ChildId == testChild1 & c.ClassId == testClassroom);
            Assert.True(child1);    
            bool child2 = list.Any(c => c.FirstName == "mette" && c.LastName == "nielsen" & c.ChildId == testChild2 & c.ClassId == testClassroom);
            Assert.True(child2);              
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete class 
            await connection.ExecuteAsync(
                "DELETE FROM classroom WHERE id = @Id",
                new { Id = testClassroom }
            );

            // Delete Child relation 
            await connection.ExecuteAsync(
                "DELETE FROM child_relation WHERE id = @Id",
                new { Id = testParent }
            );

            // Delete parent 
            await connection.ExecuteAsync(
                "DELETE FROM users WHERE id = @Id",
                new { Id = testParent }
            );

            // Delete the children users
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChild1 }
            );
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChild2 }
            );
        }
    }

    // Child with Class and parent role_approved = false  
    [Fact]
    public async Task GetAllChildrenClassesAsync_RoleApprovedFalse_ReturnChildrenWithClasses()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // Stores the temporary children id's 
        int testChildTrue, testChildFalse, testClassroom, testParentTrue, testParentFalse, testRelation1, testRelation2; 

        // temporary data are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            // Temporary classroom 
            testClassroom = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO classroom 
                    (class_name)
                VALUES 
                    ('Math') 
                RETURNING class_id
                """
            );

            // Temporary children users 
            testChildTrue = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name, class_id)
                VALUES
                    ('anna', 'nielsen', @ClassId)
                RETURNING id
                """,
                new {ClassId = testClassroom}
            );
            testChildFalse = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name, class_id)
                VALUES
                    ('mette', 'nielsen', @ClassId)
                RETURNING id
                """,
                new {ClassId = testClassroom}
            );

            // Temporary parents
            testParentTrue = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO users 
                    (first_name, last_name, email, password, role, role_approved) 
                VALUES 
                    ('henriette', 'nielsen', 'henriette@nielsen.dk', 'test123', 'Parent', 'true') 
                RETURNING id"
                """
            );
            testParentFalse = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO users 
                    (first_name, last_name, email, password, role, role_approved) 
                VALUES 
                    ('jacob', 'christensen', 'jacob@christensen.dk', 'test123', 'Parent', 'false') 
                RETURNING id"
                """
            );

            // Give parent to children
            testRelation1 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO child_relation
                    (user_id, child_id) 
                VALUES (@UserId, @ChildId) 
                ON CONFLICT DO NOTHING"
                """,
                new {UserId = testParentTrue, ChildId = testChildTrue}
            );
            testRelation2 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO child_relation
                    (user_id, child_id) 
                VALUES (@UserId, @ChildId) 
                ON CONFLICT DO NOTHING"
                """,
                new {UserId = testParentFalse, ChildId = testChildFalse}
            );            
        }
        try
        {
            // Act
            // Retrieve the children users 
            var result = await repository.GetAllChildrenClassesAsync();
            var list = result.ToList(); 

            // Assert
            // Verify that children were returned and values match 
            Assert.NotNull(result);

            // Should only be child with parent role_approved = true 
            bool childTrue = list.Any(c => c.FirstName == "anna" && c.LastName == "nielsen" & c.ChildId == testChildTrue & c.ClassId == testClassroom);
            Assert.True(childTrue);    
            bool childFalse = list.Any(c => c.FirstName == "mette" && c.LastName == "nielsen" & c.ChildId == testChildFalse & c.ClassId == testClassroom);
            Assert.False(childFalse);              
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete class 
            await connection.ExecuteAsync(
                "DELETE FROM classroom WHERE id = @Id",
                new { Id = testClassroom }
            );

            // Delete Child relation 
            await connection.ExecuteAsync(
                "DELETE FROM child_relation WHERE id = @Id",
                new { Id = testParentTrue }
            ); 
            await connection.ExecuteAsync(
                "DELETE FROM child_relation WHERE id = @Id",
                new { Id = testParentFalse }
            );

            // Delete parent 
            await connection.ExecuteAsync(
                "DELETE FROM users WHERE id = @Id",
                new { Id = testParentTrue }
            );
            await connection.ExecuteAsync(
                "DELETE FROM users WHERE id = @Id",
                new { Id = testParentFalse }
            );

            // Delete the children users
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChildTrue }
            );
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChildFalse }
            );
        }
    }

    // Child with class, but no parent 
    [Fact]
    public async Task GetAllChildrenClassesAsync_NoParent_ReturnChildrenWithClasses()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // Stores the temporary children id's 
        int testChildTrue, testChildFalse, testClassroom, testParent, testRelation; 

        // temporary data are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            // Temporary classroom 
            testClassroom = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO classroom 
                    (class_name)
                VALUES 
                    ('Math') 
                RETURNING class_id
                """
            );

            // Temporary children users 
            testChildTrue = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name, class_id)
                VALUES
                    ('anna', 'nielsen', @ClassId)
                RETURNING id
                """,
                new {ClassId = testClassroom}
            );
            testChildFalse = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name, class_id)
                VALUES
                    ('mette', 'nielsen', @ClassId)
                RETURNING id
                """,
                new {ClassId = testClassroom}
            );

            // Temporary parent
            testParent = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO users 
                    (first_name, last_name, email, password, role, role_approved) 
                VALUES 
                    ('henriette', 'nielsen', 'henriette@nielsen.dk', 'test123', 'Parent', 'true') 
                RETURNING id"
                """
            );

            // Give parent to children
            testRelation = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO child_relation
                    (user_id, child_id) 
                VALUES (@UserId, @ChildId) 
                ON CONFLICT DO NOTHING"
                """,
                new {UserId = testParent, ChildId = testChildTrue}
            ); 
        }
        try
        {
            // Act
            // Retrieve the children users 
            var result = await repository.GetAllChildrenClassesAsync();
            var list = result.ToList(); 

            // Assert
            // Verify that children were returned and values match 
            Assert.NotNull(result);

            // Should only be child with parent role_approved = true 
            bool childTrue = list.Any(c => c.FirstName == "anna" && c.LastName == "nielsen" & c.ChildId == testChildTrue & c.ClassId == testClassroom);
            Assert.True(childTrue);    
            bool childFalse = list.Any(c => c.FirstName == "mette" && c.LastName == "nielsen" & c.ChildId == testChildFalse & c.ClassId == testClassroom);
            Assert.False(childFalse);              
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete class 
            await connection.ExecuteAsync(
                "DELETE FROM classroom WHERE id = @Id",
                new { Id = testClassroom }
            );

            // Delete Child relation 
            await connection.ExecuteAsync(
                "DELETE FROM child_relation WHERE id = @Id",
                new { Id = testParent }
            ); 

            // Delete parent 
            await connection.ExecuteAsync(
                "DELETE FROM users WHERE id = @Id",
                new { Id = testParent }
            );

            // Delete the children users
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChildTrue }
            );
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChildFalse }
            );
        }
    }

    // Child without class, but parent role_approved = true 
    [Fact]
    public async Task GetAllChildrenClassesAsync_NoClass_ReturnChildrenWithClasses()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // Stores the temporary children id's 
        int testChild, testParent, testRelation; 

        // temporary data are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            // Temporary children users 
            testChild = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name)
                VALUES
                    ('anna', 'nielsen')
                RETURNING id
                """
            );

            // Temporary parent
            testParent = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO users 
                    (first_name, last_name, email, password, role, role_approved) 
                VALUES 
                    ('henriette', 'nielsen', 'henriette@nielsen.dk', 'test123', 'Parent', 'true') 
                RETURNING id"
                """
            );

            // Give parent to children
            testRelation = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO child_relation
                    (user_id, child_id) 
                VALUES (@UserId, @ChildId) 
                ON CONFLICT DO NOTHING"
                """,
                new {UserId = testParent, ChildId = testChild}
            ); 
        }
        try
        {
            // Act
            // Retrieve the child users 
            var result = await repository.GetAllChildrenClassesAsync();

            // Assert
            // Verify that child were returned and values match 
            Assert.NotNull(result);

            // classId should be null, as child does not have a class
            bool child = result.Any(c => c.FirstName == "anna" && c.LastName == "nielsen" & c.ChildId == testChild & c.ClassId == null);
            Assert.True(child);             
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete Child relation 
            await connection.ExecuteAsync(
                "DELETE FROM child_relation WHERE id = @Id",
                new { Id = testParent }
            ); 

            // Delete parent 
            await connection.ExecuteAsync(
                "DELETE FROM users WHERE id = @Id",
                new { Id = testParent }
            );

            // Delete the child users
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChild }
            );

        }
    }

    // ----- INSERT CHILD ----- 
    // All attributes in Children entity 
    [Fact]
    public async Task InsertAsync_ReturnChildId()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // temporary child entity 
        Children testChild;
        // Generate random number 
        Random rnd = new Random();

        // temporary data are inserted into the test database
        int id = rnd.Next();
        // Set the child entity
        testChild = new Children{
            ChildId = id, 
            FirstName = "Jenny",
            LastName = "Thomsen",
            ClassId = 2};
        try
        {
            // Act
            // Insert the children entity into database 
            var result = await repository.InsertAsync(testChild);

            // Assert
            // returned id should match the provided child id 
            Assert.Equal(id, result);            
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete the inserted child
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = id }
            ); 

        }
    }

    // Without ChildId attribute
    [Fact]
    public async Task InsertAsync_WithoutChildId_ThrowsException()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // temporary child entity 
        Children testChild;

        // Set the child entity
        testChild = new Children{
            FirstName = "Jenny",
            LastName = "Thomsen",
            ClassId = 2};
        
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
        () => repository.InsertAsync(testChild)
        );
        
    }

    // Without name attributes 
    [Fact]
    public async Task InsertAsync_WithoutNames_ThrowsException()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // temporary child entity 
        Children testChild;

        // Set the child entity
        testChild = new Children{
            ChildId = 1,
            ClassId = 2
        };
        
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
        () => repository.InsertAsync(testChild)
        );
        
    }

    // Without classId attribute 
    [Fact]
    public async Task InsertAsync_WithoutClassId_ReturnChildId()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // temporary child entity 
        Children testChild;

        // temporary data are inserted into the test database
        // Set the child entity
        testChild = new Children{
            ChildId = 1, 
            FirstName = "Jenny",
            LastName = "Thomsen"};
        try
        {
            // Act
            // Insert the children entity into database 
            var result = await repository.InsertAsync(testChild);
            Children resultChild;
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                // retrieves the inserted child
                resultChild = await connection.QuerySingleAsync<Children>(
                    @"SELECT child_id AS ChildId,
                    first_name AS FirstName,
                    last_name AS LastName,
                    class_id AS ClassId
                    FROM children
                    WHERE child_id = @ChildId",
                    new { ChildId = 1 }
                ); 
            }
        
            // Assert
            // returned id should match the provided child id 
            Assert.Equal(1, result);  
            // ClassId default should be set as null    
            Assert.Null(resultChild.ClassId);
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete the inserted child
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = 1"
            ); 

        }
    }

    // ----- UPDATE ----- 
    // With all attributes in child entity, multiple children in database 
    [Fact]
    public async Task UpdateAsync_Return1()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        // temporary variable
        int testChildId1, testChildId2;
        Children testChild1;

        // temporary data are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            // Temporary children users 
            testChildId1 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name)
                VALUES
                    ('tony', 'poulsen')
                RETURNING id
                """
            );
            testChildId2 = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO children
                    (first_name, last_name)
                VALUES
                    ('ingrid', 'nielsen')
                RETURNING id
                """
            );

            // Create new temporary children entity to be updated
            testChild1 = new Children{
              ChildId = testChildId1, 
              FirstName = "tina", 
              LastName = "torgesen",
              ClassId = 2  
            };
        }
        try
        {
            // Act
            // Update the children entity into database 
            var result = await repository.UpdateAsync(testChild1);
            // Retrieve child entity form database 
            Children resultChild;
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                // retrieves the inserted child
                resultChild = await connection.QuerySingleAsync<Children>(
                    @"SELECT child_id AS ChildId,
                    first_name AS FirstName,
                    last_name AS LastName,
                    class_id AS ClassId
                    FROM children
                    WHERE child_id = @ChildId",
                    new { ChildId = testChildId1 }
                ); 
            }

            // Assert
            // result should be 1, as only one entity should be updated
            Assert.Equal(1, result);
            // Child attributes should be expected values 
            Assert.Equal(testChildId1, resultChild.ChildId); 
            Assert.Equal("tina", resultChild.FirstName); 
            Assert.Equal("torgensen", resultChild.LastName);
            Assert.Equal(2, resultChild.ClassId);
        }
        finally
        {
            // Cleanup
            // The finally block ensures that test data is removed even if
            // one of the assertions above fails.

            using var connection = _connectionFactory.Create();
            connection.Open();

            // Delete the inserted childen 
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChildId1 }
            ); 
            await connection.ExecuteAsync(
                "DELETE FROM children WHERE id = @Id",
                new { Id = testChildId2 }
            ); 
        }
    }

    // With all attributes in child entity, child not in the database
    [Fact]
    public async Task UpdateAsync_Return0()
    {
        // Arrange
        // Create the repository using the test database connection.
        var repository = new ChildrenRepository(_connectionFactory);

        Children testChild; 

        // temporary data are inserted into the test database
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();

            // Create new temporary children entity to be updated
            testChild = new Children{
              ChildId = 1, 
              FirstName = "tina", 
              LastName = "torgesen",
              ClassId = 2  
            };
        }
        // Act
        // Update the children entity into database 
        var result = await repository.UpdateAsync(testChild);

        // Assert
        // result should be 0, as no entity should be updated
        Assert.Equal(0, result);
    }

    // With all attributes in child entity, multiple children in database with same ID 
    [Fact]
    
    // Without ChildId attribute
    

    // Without names 

    // Without class id 

    
}