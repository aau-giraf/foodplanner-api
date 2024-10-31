using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(20243110)]
public class CreateUsersTable : Migration {
    
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("id").AsInt32().PrimaryKey().NotNullable()
            .WithColumn("first_name").AsString(30).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("email").AsString(100).NotNullable()
            .WithColumn("password").AsString(100).NotNullable()
            .WithColumn("role").AsString(30).NotNullable()
            .WithColumn("role_approved").AsString().NotNullable()
            .WithColumn("pincode").AsString(100).NotNullable();

    }

    public override void Down() 
    {
        Delete.Table("users");
    }
    
}