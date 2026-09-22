using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

//Intitial databse setup (aka migration 1)
[Migration(1)]
public class InitTables : Migration {
/* Up() is a function from the FluentMigrator framework
   that defines the changes applied when the migration is run */  
public override void Up() 
{
    //Creates table "users"
    Create.Table("users")
        .WithColumn("id").AsInt32().PrimaryKey().NotNullable() //primary key that does not auto-increment
        .WithColumn("first_name").AsString(30).NotNullable()
        .WithColumn("last_name").AsString(100).NotNullable()
        .WithColumn("email").AsString(100).NotNullable()
        .WithColumn("password").AsString(100).NotNullable()
        .WithColumn("role").AsString(30).NotNullable()
        .WithColumn("role_approved").AsString().NotNullable()
        .WithColumn("pincode").AsString(100).NotNullable();
    
    //Creates table "classroom"
    Create.Table("classroom")
        .WithColumn("class_id").AsInt32().PrimaryKey().Identity() //primary key that auto-increments
        .WithColumn("class_name").AsString().NotNullable();
    
    //create table "Children"
    Create.Table("children")
        .WithColumn("child_id").AsInt32().PrimaryKey().Identity().NotNullable() //Primary key that auto-increments
        .WithColumn("first_name").AsString(100).NotNullable()
        .WithColumn("last_name").AsString(100).NotNullable()
        .WithColumn("parent_id").AsInt32().NotNullable() //foreign key pointing at users.id
        .WithColumn("class_id").AsInt32().NotNullable(); //foreign key pointing at classroom.class_id

    //Creates a foreign key from "children.parent_id" that points to "users.id"
    Create.ForeignKey("fk_children_parent_id")
        .FromTable("children").ForeignColumn("parent_id")
        .ToTable("users").PrimaryColumn("id");

    //Creates a foreign key from "children.class_id" that points to "classroom.class_id"
    Create.ForeignKey("fk_children_class_id")
        .FromTable("children").ForeignColumn("class_id")
        .ToTable("classroom").PrimaryColumn("class_id");
    
    //Creates table "food_image"
    Create.Table("food_image")
        .WithColumn("id").AsInt32().PrimaryKey().Identity() //primary key, auto incrementing
        .WithColumn("image_id").AsString().NotNullable()
        .WithColumn("user_id").AsInt32().NotNullable()
        .WithColumn("image_name").AsString().NotNullable()
        .WithColumn("image_file_type").AsString().NotNullable()
        .WithColumn("size").AsInt64().NotNullable();
}
    /* Down() is a function from the FluentMigrator framework
    that defines how the migration is rolled back */
    public override void Down()
    {
        //Deletes the tables created by this migration
        Delete.Table("users");
        Delete.Table("classroom");
        Delete.Table("children");
        Delete.Table("food_image");
    }
    
}