using FluentMigrator;
using Microsoft.Extensions.Primitives;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(3)]
public class addedNewTables: Migration
{
    public override void Up()
    {
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
        Delete.Table("meals");
        Delete.Table("ingredients");
        Delete.Table("packed_ignredients");
    }
}