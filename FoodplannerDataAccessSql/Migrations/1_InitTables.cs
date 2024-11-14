using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;


[Migration(1)]
public class InitTables : Migration {
    
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
    
    Create.Table("classroom")
        .WithColumn("class_id").AsInt32().PrimaryKey().Identity()
        .WithColumn("class_name").AsString().NotNullable();
    
    Create.Table("children")
        .WithColumn("child_id").AsInt32().PrimaryKey().Identity().NotNullable()
        .WithColumn("first_name").AsString(100).NotNullable()
        .WithColumn("last_name").AsString(100).NotNullable()
        .WithColumn("parent_id").AsInt32().NotNullable()
        .WithColumn("class_id").AsInt32().NotNullable();

    Create.ForeignKey("fk_children_parent_id")
        .FromTable("children").ForeignColumn("parent_id")
        .ToTable("users").PrimaryColumn("id");

    Create.ForeignKey("fk_children_class_id")
        .FromTable("children").ForeignColumn("class_id")
        .ToTable("classroom").PrimaryColumn("class_id");
    
    Create.Table("food_image")
        .WithColumn("id").AsInt32().PrimaryKey().Identity()
        .WithColumn("image_id").AsString().NotNullable()
        .WithColumn("user_id").AsInt32().NotNullable()
        .WithColumn("image_name").AsString().NotNullable()
        .WithColumn("image_file_type").AsString().NotNullable()
        .WithColumn("size").AsInt64().NotNullable();
}

    public override void Down()
    {
        Delete.Table("users");
        Delete.Table("classroom");
        Delete.Table("children");
        Delete.Table("food_image");
    }
    
}