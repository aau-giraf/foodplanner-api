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

        Create.Table("meals")
        .WithColumn("id").AsInt32().PrimaryKey().Identity()
        .WithColumn("title").AsString(100).NotNullable()
        .WithColumn("date").AsString(100).NotNullable()
        .WithColumn("image_ref").AsInt32().Nullable()
        .WithColumn("user_ref").AsInt32().NotNullable();

    Create.ForeignKey("fk_meals_user_ref")
        .FromTable("meals").ForeignColumn("user_ref")
        .ToTable("users").PrimaryColumn("id");
    
    Create.ForeignKey("fk_meals_image_ref")
        .FromTable("meals").ForeignColumn("image_ref")
        .ToTable("food_image").PrimaryColumn("id");
    
    Create.Table("ingredients")
        .WithColumn("id").AsInt32().PrimaryKey().Identity()
        .WithColumn("name").AsString(100).NotNullable()
        .WithColumn("image_ref").AsInt32().Nullable()
        .WithColumn("user_ref").AsInt32().NotNullable();
    
    Create.ForeignKey("fk_ingredients_user_ref")
        .FromTable("ingredients").ForeignColumn("user_ref")
        .ToTable("users").PrimaryColumn("id");

    Create.ForeignKey("fk_ingredients_image_ref")
        .FromTable("ingredients").ForeignColumn("image_ref")
        .ToTable("food_image").PrimaryColumn("id");
    
    Create.Table("packed_ingredients")
        .WithColumn("id").AsInt32().PrimaryKey().Identity()
        .WithColumn("meal_ref").AsInt32().NotNullable()
        .WithColumn("ingredient_ref").AsInt32().NotNullable();
    
    Create.ForeignKey("fk_packed_ingredients_meal_ref")
        .FromTable("packed_ingredients").ForeignColumn("meal_ref")
        .ToTable("meals").PrimaryColumn("id");

    Create.ForeignKey("fk_packed_ingredients_ingredient_ref")
        .FromTable("packed_ingredients").ForeignColumn("ingredient_ref")
        .ToTable("ingredients").PrimaryColumn("id");
}

    public override void Down()
    {
        Delete.Table("users");
        Delete.Table("classroom");
        Delete.Table("children");
        Delete.Table("food_image");
        Delete.Table("meals");
        Delete.Table("ingredients");
        Delete.Table("packed_ignredients");
    }
    
}